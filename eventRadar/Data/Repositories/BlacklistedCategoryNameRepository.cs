using eventRadar.Controllers;
using eventRadar.Models;
using eventRadar;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IBlacklistedCategoryNameRepository
    {
        Task CreateAsync(BlacklistedCategoryName categoryName);
        Task DeleteAsync(BlacklistedCategoryName categoryName);
        Task UpdateAsync(BlacklistedCategoryName categoryName);
        Task<BlacklistedCategoryName?> GetAsync(int blacklistedCategoryId);
        Task<IReadOnlyList<BlacklistedCategoryName>> GetManyAsync();
    }
    public class BlacklistedCategoryNameRepository : IBlacklistedCategoryNameRepository
    {
        private readonly WebDbContext _webDbContext;
        public BlacklistedCategoryNameRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<IReadOnlyList<BlacklistedCategoryName>> GetManyAsync()
        {
            return await _webDbContext.BlacklistedCategoryNames.ToListAsync();
        }
        public async Task<BlacklistedCategoryName?> GetAsync(int blacklistedCategoryId)
        {
            return await _webDbContext.BlacklistedCategoryNames.FirstOrDefaultAsync(o => o.Id == blacklistedCategoryId);
        }
        public async Task CreateAsync(BlacklistedCategoryName blacklistedCategoryName)
        {
            _webDbContext.BlacklistedCategoryNames.Add(blacklistedCategoryName);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(BlacklistedCategoryName blacklistedCategoryName)
        {
            _webDbContext.BlacklistedCategoryNames.Add(blacklistedCategoryName);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(BlacklistedCategoryName blacklistedCategoryName)
        {
            _webDbContext.BlacklistedCategoryNames.Remove(blacklistedCategoryName);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
