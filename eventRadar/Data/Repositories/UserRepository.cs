using eventRadar.Auth.Model;
using eventRadar.Controllers;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<User?> GetAsync(int userId);
        Task<IReadOnlyList<User>> GetManyAsync();
    }
    public class UserRepository : IUserRepository
    {
        private readonly WebDbContext _webDbContext;
        public UserRepository(WebDbContext webDbContext)
        { 
            _webDbContext = webDbContext;
        }
        public async Task<User?> GetAsync(int userId)
        {
            return await _webDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        }
        public async Task<IReadOnlyList<User>> GetManyAsync()
        {
            return await _webDbContext.Users.ToListAsync();
        }
        public async Task CreateAsync(User user)
        {
            _webDbContext.Users.Add(user);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(User user)
        {
            _webDbContext.Users.Update(user);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(User user)
        {
            _webDbContext.Users.Remove(user);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
