using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IChangedEventRepository
    {
        Task CreateAsync(ChangedEvent changedEvent);
        Task UpdateAsync(ChangedEvent changedEvent);
        Task DeleteAsync(ChangedEvent changedEvent);
        Task<ChangedEvent?> GetAsync(string changedEventId);
        Task<IReadOnlyList<ChangedEvent>> GetManyAsync();
    }
    public class ChangedEventRepository : IChangedEventRepository
    {
        private readonly WebDbContext _webDbContext;
        public ChangedEventRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<ChangedEvent?> GetAsync(string changedEventId)
        {
            return await _webDbContext.ChangedEvents.FirstOrDefaultAsync(o => o.Id == changedEventId);
        }
        public async Task<IReadOnlyList<ChangedEvent>> GetManyAsync()
        {
            return await _webDbContext.ChangedEvents.ToListAsync();
        }
        public async Task CreateAsync(ChangedEvent changedEvent)
        {
            _webDbContext.ChangedEvents.Add(changedEvent);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync (ChangedEvent changedEvent)
        {
            _webDbContext.ChangedEvents.Update(changedEvent);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync (ChangedEvent changedEvent)
        {
            _webDbContext.ChangedEvents.Remove(changedEvent);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
