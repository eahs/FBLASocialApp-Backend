using ADSBackend.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Controllers.Api.v1
{
    [Produces("application/json")]
    [Route("api/v1/Members")]
    public class MembersController : Controller
    {
        private readonly Services.Configuration Configuration;
        private readonly Services.Cache _cache;

        public MembersController(Services.Configuration configuration, Services.Cache cache)
        {
            Configuration = configuration;
            _cache = cache;
        }

        // GET: api/v1/Members/{id}
        /// <summary>
        /// Returns a specific member
        /// </summary>
        /// <param name="id"></param>    
        [HttpGet("{id}")]
        public async Task<bool> GetMember(int id)
        {
            return true;  // Should return Task<Member>
        }

        // POST: api/v1/Members/
        /// <summary>
        /// Create a new member
        /// </summary>
        /// <param name="item"></param>   
        [HttpPost]
        public async Task<NewsFeedItem> CreateMember (NewsFeedItem item)
        {
            return item;
        }

        // PUT: api/v1/Members/{id}
        /// <summary>
        /// Update an existing member
        /// </summary>
        /// <param name="id"></param>   
        /// <param name="item"></param>   
        [HttpPut("{id}")]
        public async Task<NewsFeedItem> UpdateMember(int id, NewsFeedItem item)
        {
            return item;
        }

        // DELETE: api/v1/Members/{id}
        /// <summary>
        /// Delete a member
        /// </summary>
        /// <param name="id"></param>   
        [HttpDelete("{id}")]
        public async Task<bool> DeleteMember(int id)
        {
            return true;
        }
        

    }
}