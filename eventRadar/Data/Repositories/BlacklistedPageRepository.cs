using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IBlacklistedPageRepository
    {
        Task CreateAsync(BlacklistedPage blacklistedPage);
        Task DeleteAsync(BlacklistedPage blacklistedPage);
        Task<BlacklistedPage?> GetAsync(string blacklistePageID);
        Task<IReadOnlyList<BlacklistedPage>> GetManyAsync();
        Task UpdateAsync (BlacklistedPage blacklistedPage);
    }
    public class BlacklistedPageRepository : IBlacklistedPageRepository
    {
        private readonly WebDbContext _webDbContext;
        public BlacklistedPageRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<BlacklistedPage?> GetAsync (string blacklistedPageId)
        {
            return await _webDbContext.BlacklistedPages.FirstOrDefaultAsync(o => o.Id == blacklistedPageId);
        }
        public async Task<IReadOnlyList<BlacklistedPage>> GetManyAsync()
        {
            return await _webDbContext.BlacklistedPages.ToListAsync();
        }
        public async Task CreateAsync(BlacklistedPage blacklistedPage)
        {
            _webDbContext.BlacklistedPages.Add(blacklistedPage);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(BlacklistedPage blacklistedPage)
        {
            _webDbContext.BlacklistedPages.Update(blacklistedPage);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(BlacklistedPage blacklistedPage)
        {
            _webDbContext.BlacklistedPages.Remove(blacklistedPage);
            await _webDbContext.SaveChangesAsync();
        }
    }

}
