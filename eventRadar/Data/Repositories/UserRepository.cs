using eventRadar.Auth.Model;
using eventRadar.Controllers;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IUserRepository
    {
        Task BlockAsync(User user);
        Task<User> GetAsync(string userId);
        Task<IReadOnlyList<User>> GetManyAsync();
    }
    public class UserRepository : IUserRepository
    {
        private readonly WebDbContext _webDbContext;
        public UserRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<User> GetAsync(string userId)
        {
            return await _webDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        }
        public async Task<IReadOnlyList<User>> GetManyAsync()
        {
            return await _webDbContext.Users.ToListAsync();
        }
        public async Task BlockAsync(User user)
        {
            _webDbContext.Users.Update(user);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
