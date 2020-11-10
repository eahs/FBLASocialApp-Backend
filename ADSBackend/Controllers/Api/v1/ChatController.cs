using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADSBackend.Controllers.Api.v1;
using ADSBackend.Data;
using ADSBackend.Models;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using ADSBackend.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using ADSBackend.Hubs;

namespace ADSBackend.Controllers.Api.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/v1/chat")]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(ApplicationDbContext context, IUserService userService, IHubContext<ChatHub> hub)
        {
            _context = context;
            _userService = userService;
            _hubContext = hub;
        }

        [HttpGet]
        public async Task<ApiResponse> GetActiveSessions()
        {
            var httpUser = (Member)HttpContext.Items["User"];

            var member = await _context.Member.Where(m => m.MemberId == httpUser.MemberId)
                                              .Include(m => m.ChatSessions).ThenInclude(m => m.ChatSession).ThenInclude(m => m.ChatMembers)
                                              .FirstOrDefaultAsync();

            if (member != null)
            {
                return new ApiResponse(System.Net.HttpStatusCode.OK, member.ChatSessions.Select(cs => cs.ChatSession).ToList());
            }

            return new ApiResponse(System.Net.HttpStatusCode.NotFound, null, errorMessage: "No active sessions found");
        }

        /// <summary>
        /// Connect a member to stream to their existing chat sessions
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        [HttpPost("connect")]
        public async Task<ApiResponse> ConnectToChatSessions(string connectionId)
        {
            var gas = await GetActiveSessions();

            if (gas.StatusCode == 200)
            {
                var sessions = (List<ChatSession>)gas.Result;

                foreach (var session in sessions)
                {
                    await _hubContext.Groups.AddToGroupAsync(connectionId, session.ChatPrivateKey);
                }

                new ApiResponse(System.Net.HttpStatusCode.OK, null);
            }

            return new ApiResponse(System.Net.HttpStatusCode.NotFound, null, errorMessage: "No active sessions found");
        }

        /// <summary>
        /// Create a new chat session between several participants
        /// </summary>
        /// <param name="participants"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse> CreateChatSession(List<int> participants)
        {
            var httpUser = (Member)HttpContext.Items["User"];

            if (participants == null || participants.Count == 0)
            {
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, errorMessage: "You must have at least one participant");
            }


            var member = await _context.Member.Where(m => m.MemberId == httpUser.MemberId)
                                              .Include(m => m.Friends)
                                              .FirstOrDefaultAsync();

            // Let's grab the list of friends that are in the participants list
            var chatgroup = member.Friends.Where(f => participants.Contains(f.FriendId)).Select(f => f.FriendId).ToList();

            if (chatgroup.Count == 0)
            {
                return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, errorMessage: "You can only chat with existing friends");
            }

            chatgroup.Add(httpUser.MemberId);

            var chatSession = new ChatSession
            {
                ChatPrivateKey = System.Guid.NewGuid().ToString("N")
            };

            _context.ChatSession.Add(chatSession);
            await _context.SaveChangesAsync();

            var chatMembers = new List<ChatSessionMember>();
            foreach (var memberId in chatgroup)
            {
                chatMembers.Add(new ChatSessionMember
                {
                    ChatSessionId = chatSession.ChatSessionId,
                    MemberId = memberId
                });
            }

            chatSession.ChatMembers = chatMembers;
            _context.ChatSession.Update(chatSession);
            await _context.SaveChangesAsync();

            return new ApiResponse(System.Net.HttpStatusCode.OK, chatSession);
        }

        /// <summary>
        /// Get the chat session denoted by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetChatSession(int id)
        {
            var sessionResponse = await GetSessionHelper(id);
            if (sessionResponse.StatusCode != (int)System.Net.HttpStatusCode.OK)
                return sessionResponse;

            var session = (ChatSession)sessionResponse.Result;

            return new ApiResponse(System.Net.HttpStatusCode.OK, session);
        }

        /// <summary>
        /// Add a member by memberId to a chat session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpPost("members/{id}")]
        public async Task<ApiResponse> AddSessionMember(int id, int memberId)
        {
            var sessionResponse = await GetSessionHelper(id);
            if (sessionResponse.StatusCode != (int)System.Net.HttpStatusCode.OK)
                return sessionResponse;

            var session = (ChatSession)sessionResponse.Result;

            var exists = session.ChatMembers.FirstOrDefault(cm => cm.MemberId == memberId);

            if (exists == null)
            {
                session.ChatMembers.Add(new ChatSessionMember
                {
                    ChatSessionId = session.ChatSessionId,
                    MemberId = memberId
                });

                _context.ChatSession.Update(session);
                await _context.SaveChangesAsync();
            }

            return new ApiResponse(System.Net.HttpStatusCode.OK, session);
        }

        /// <summary>
        /// Removes a member from a chat session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpDelete("members/{id}")]
        public async Task<ApiResponse> RemoveSessionMember(int id, int memberId)
        {
            var sessionResponse = await GetSessionHelper(id);
            if (sessionResponse.StatusCode != (int)System.Net.HttpStatusCode.OK)
                return sessionResponse;

            var session = (ChatSession)sessionResponse.Result;

            session.ChatMembers.RemoveAll(cm => cm.MemberId == memberId);

            _context.ChatSession.Update(session);
            await _context.SaveChangesAsync();

            return new ApiResponse(System.Net.HttpStatusCode.OK, session);
        }

        /// <summary>
        /// Adds a new message to a chat
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <param name="connectionId">optional signalR connection id</param>
        /// <returns></returns>
        [HttpPost("messages/{id}")]
        public async Task<ApiResponse> AddMessage(int id, [Bind("AuthorId,Body,ChatSessionId")]ChatMessage message, string connectionId = null)
        {
            var sessionResponse = await GetSessionHelper(id);
            if (sessionResponse.StatusCode != (int)System.Net.HttpStatusCode.OK)
                return sessionResponse;

            var session = (ChatSession)sessionResponse.Result;

            message.ChatSessionId = session.ChatSessionId;
            message.CreatedAt = DateTime.Now;
            message.EditedAt = DateTime.Now;
            message.IsDeleted = false;

            session.Messages.Add(message);

            _context.ChatSession.Update(session);
            await _context.SaveChangesAsync();

            // If a connectionId is specified, add that user to the appropriate group
            if (connectionId != null)
            {
                await _hubContext.Groups.AddToGroupAsync(connectionId, session.ChatPrivateKey);
            }

            await SendGroupMessage(message);
            
            return new ApiResponse(System.Net.HttpStatusCode.OK, message);
        }


        private async Task<ApiResponse> GetSessionHelper (int sessionId)
        {
            var httpUser = (Member)HttpContext.Items["User"];

            var session = await _context.ChatSession.Where(s => s.ChatSessionId == sessionId)
                .Include(p => p.ChatMembers)
                .ThenInclude(m => m.Member)
                .FirstOrDefaultAsync();

            if (session == null)
            {
                return new ApiResponse(System.Net.HttpStatusCode.NotFound, null, errorMessage: "Chat session does not exist");
            }

            var member = session.ChatMembers?.FirstOrDefault(m => m.MemberId == httpUser.MemberId);

            if (member == null)
            {
                return new ApiResponse(System.Net.HttpStatusCode.Forbidden, null, errorMessage: "User is not a participant in this chat session - Access is denied");
            }

            return new ApiResponse(System.Net.HttpStatusCode.OK, session);
        }

        private async Task<bool> SendGroupMessage(ChatMessage message)
        {
            await _hubContext.Clients.Group(message.ChatSession.ChatPrivateKey).SendAsync("Receive", message);

            return true;
        }

    }
}
