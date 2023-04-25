using eventRadar.Controllers;
using eventRadar.Models;
using eventRadar;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task CreateAsync(Category category);
        Task DeleteAsync(Category category);
        Task UpdateAsync(Category category);
        Task<Category?> GetAsync(string categoryId);
        Task<IReadOnlyList<Category>> GetManyAsync();
    }
    public class CategoryRepository : ICategoryRepository
    {
        private readonly WebDbContext _webDbContext;
        public CategoryRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<Category?> GetAsync(string categoryId)
        {
            return await _webDbContext.Categories.FirstOrDefaultAsync(o => o.Id == categoryId);
        }
        public async Task<IReadOnlyList<Category>> GetManyAsync()
        {
            return await _webDbContext.Categories.ToListAsync();
        }
        public async Task CreateAsync(Category category)
        {
            _webDbContext.Categories.Add(category);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(Category category)
        {
            _webDbContext.Categories.Update(category);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Category category)
        {
            _webDbContext.Categories.Remove(category);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
