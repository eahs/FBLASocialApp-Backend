using ADSBackend.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Controllers.Api.v1
{
    [Produces("application/json")]
    [Route("api/v1/news")]
    public class NewsController : Controller
    {
        private readonly Services.Configuration Configuration;
        private readonly Services.Cache _cache;

        public NewsController(Services.Configuration configuration, Services.Cache cache)
        {
            Configuration = configuration;
            _cache = cache;
        }

        // GET: api/News
        [HttpGet]
        public async Task<List<NewsFeedItem>> GetFeed()
        {
            var newsUrl = new Uri("https://www.eastonsd.org/apps/news/news_rss.jsp");

            string sourceUrl = newsUrl.GetLeftPart(UriPartial.Authority);
            string endpoint = newsUrl.PathAndQuery;

            Task<List<NewsFeedItem>> fetchNewsFromSource() => Util.RSS.GetNewsFeed(sourceUrl, endpoint);

            var feedItems = await _cache.GetAsync("RSS", fetchNewsFromSource, TimeSpan.FromMinutes(5));
            return feedItems.OrderByDescending(x => x.PublishDate).ToList();
        }

        
        [HttpPost]
        public async Task<NewsFeedItem> CreateNewsFeedItem (NewsFeedItem item)
        {
            return item;
        }

        [HttpPut("{id}")]
        public async Task<NewsFeedItem> UpdateNewsFeedItem(int id, NewsFeedItem item)
        {
            return item;
        }

        [HttpDelete("{id}")]
        public async Task<bool> DeleteNewsFeedItem(int id)
        {
            return true;
        }
        

        // GET: api/v1/News/Config
        [HttpGet("Config")]
        public ConfigResponse GetConfig()
        {
            // TODO: extend this object to include some configuration items
            return new ConfigResponse();
        }
    }
}