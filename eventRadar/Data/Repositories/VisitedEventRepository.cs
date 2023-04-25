using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IVisitedEventRepository
    {
        Task CreateAsync(VisitedEvent visitedEvent);
        Task DeleteAsync(VisitedEvent visitedEvent);
        Task<VisitedEvent?> GetAsync(User user, int visitedEventId);
        Task<IReadOnlyList<VisitedEvent>> GetManyAsync(User user);
    }
    public class VisitedEventRepository : IVisitedEventRepository
    {
        private readonly WebDbContext _webDbContext;

        public VisitedEventRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<VisitedEvent?> GetAsync(User user, int visitedEventId)
        {
            return await _webDbContext.VisitedEvents.Where(o => o.User == user).FirstOrDefaultAsync(o => o.Id == visitedEventId);
        }
        public async Task<IReadOnlyList<VisitedEvent>> GetManyAsync(User user)
        {
            return await _webDbContext.VisitedEvents.Where(o => o.User == user).ToListAsync();
        }
        public async Task CreateAsync(VisitedEvent visitedEvent)
        {
            _webDbContext.VisitedEvents.Add(visitedEvent);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(VisitedEvent visitedEvent)
        {
            _webDbContext.VisitedEvents.Remove(visitedEvent);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
