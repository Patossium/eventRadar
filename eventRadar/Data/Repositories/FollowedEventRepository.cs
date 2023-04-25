using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IFollowedEventRepository
    {
        Task CreateAsync(FollowedEvent followedEvent);
        Task DeleteAsync(FollowedEvent followedEvent);
        Task<FollowedEvent?> GetAsync(User user, int followedEventId);
        Task<IReadOnlyList<FollowedEvent>> GetManyAsync(User user);
    }
    public class FollowedEventRepository : IFollowedEventRepository
    {
        private readonly WebDbContext _webDbContext;
        public FollowedEventRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<FollowedEvent?> GetAsync(User user, int followedEventId)
        {
            return await _webDbContext.FollowedEvents.Where(o => o.User == user).FirstOrDefaultAsync( o => o.Id == followedEventId);
        }
        public async Task<IReadOnlyList<FollowedEvent>> GetManyAsync(User user)
        {
            return await _webDbContext.FollowedEvents.Where(o => o.User == user).ToListAsync();
        }
        public async Task CreateAsync(FollowedEvent followedEvent)
        {
            _webDbContext.FollowedEvents.Add(followedEvent);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(FollowedEvent followedEvent)
        {
            _webDbContext.FollowedEvents.Remove(followedEvent);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
