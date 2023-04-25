using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IFollowedUserRepository
    {
        Task CreateAsync(FollowedUser followedUser);
        Task DeleteAsync(FollowedUser followedUser);
        Task<FollowedUser?> GetAsync(User user, int followedUserID);
        Task<IReadOnlyList<FollowedUser>> GetManyAsync(User user);
    }
    public class FollowedUserRepository : IFollowedUserRepository
    {
        private readonly WebDbContext _webDbContext;
        public FollowedUserRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<FollowedUser?> GetAsync(User user, int followedUserId)
        {
            return await _webDbContext.FollowedUsers.Where(o => o.User == user).FirstOrDefaultAsync(o => o.Id == followedUserId);
        }
        public async Task<IReadOnlyList<FollowedUser>> GetManyAsync(User user)
        {
            return await _webDbContext.FollowedUsers.Where(o => o.User == user).ToListAsync();
        }
        public async Task CreateAsync(FollowedUser followedUser)
        {
            _webDbContext.FollowedUsers.Add(followedUser);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(FollowedUser followedUser)
        {
            _webDbContext.FollowedUsers.Remove(followedUser);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
