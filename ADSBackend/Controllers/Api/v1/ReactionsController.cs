using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADSBackend.Data;
using ADSBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using ADSBackend.Controllers.Api.v1;

namespace YakkaApp.Controllers.Api.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/v1/members")]
    public class ReactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reactions
        public async Task<ApiResponse> Index()
        {

        }

        // GET: Reactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {

        }

        // GET: Reactions/Create
        public Task<ApiResponse> Create()
        {

        }

        // POST: Reactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ApiResponse> Create([Bind("ReactionId,ReactionType,MemberId")] Reaction reaction)
        {

        }

        // GET: Reactions/Edit/5
        public async Task<ApiResponse> Edit(int? id)
        {
            return View(reaction);
        }

        // POST: Reactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ApiResponse> Edit(int id, [Bind("ReactionId,ReactionType,MemberId")] Reaction reaction)
        {

        }

        // GET: Reactions/Delete/5
        public async Task<ApiResponse> Delete(int? id)
        {
        }

        // POST: Reactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ApiResponse> DeleteConfirmed(int id)
        {

        }

        private bool ReactionExists(int id)
        {
            return _context.Reaction.Any(e => e.ReactionId == id);
        }
    }
}
