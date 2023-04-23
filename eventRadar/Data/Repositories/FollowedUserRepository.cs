using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IFollowedUserRepository
    {
        Task CreateAsync(FollowedUser followedUser);
        Task DeleteAsync(FollowedUser followedUser);
        Task UpdateAsync(FollowedUser followedUser);
        Task<FollowedUser?> GetAsync(int followedUserID);
        Task<IReadOnlyCollection<FollowedUser>> GetManyAsync();
    }
    public class FollowedUserRepository : IFollowedUserRepository
    {
        private readonly WebDbContext _webDbContext;
        public FollowedUserRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<FollowedUser?> GetAsync(int followedUserId)
        {
            return await _webDbContext.FollowedUsers.FirstOrDefaultAsync(o => o.Id == followedUserId);
        }
        public async Task<IReadOnlyList<FollowedUser>> GetManyAsync()
        {
            return await _webDbContext.FollowedUsers.ToListAsync()
        }
        public async Task CreateAsync(FollowedUser followedUser)
        {
            _webDbContext.FollowedUsers.Add(followedUser);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(FollowedUser followedUser)
        {
            _webDbContext.FollowedUsers.Update(followedUser);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(FollowedUser followedUser)
        {
            _webDbContext.FollowedUsers.Remove(followedUser);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
