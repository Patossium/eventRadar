using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IWebsiteRepository
    {
        Task CreateAsync(Website website);
        Task UpdateAsync(Website website);
        Task DeleteAsync(Website website);
        Task<Website?> GetAsync(int websiteId);
        Task<IReadOnlyList<Website>> GetManyAsync();
    }
    public class WebsiteRepository : IWebsiteRepository
    {
        private readonly WebDbContext _webDbContext;
        public WebsiteRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<Website?> GetAsync(int websiteId)
        {
            return await _webDbContext.Websites.FirstOrDefaultAsync(o => o.Id == websiteId);
        }
        public async Task<IReadOnlyList<Website>> GetManyAsync()
        {
            return await _webDbContext.Websites.ToListAsync();
        }
        public async Task CreateAsync(Website website)
        {
            _webDbContext.Websites.Add(website);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(Website website)
        {
            _webDbContext.Websites.Update(website);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Website website)
        {
            _webDbContext.Websites.Remove(website);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
