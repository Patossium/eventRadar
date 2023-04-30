using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;
using eventRadar.Helpers;
using eventRadar.Data.Dtos;

namespace eventRadar.Data.Repositories
{
    public interface IEventRepository
    {
        Task CreateAsync(Event eventObject);
        Task DeleteAsync(Event eventObject);
        Task<Event?> GetAsync(int eventId);
        Task<IReadOnlyList<Event>> GetManyAsync();
        Task<PagedList<Event>> GetManyAsync(EventSearchParameters eventSearchParameters);
        Task UpdateAsync(Event eventObject);
    }

    public class EventRepository : IEventRepository
    {
        private readonly WebDbContext _webDbContext;
        public EventRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<Event?> GetAsync(int eventId)
        {
            return await _webDbContext.Events.FirstOrDefaultAsync(o => o.Id == eventId);
        }
        public async Task<PagedList<Event>> GetManyAsync(EventSearchParameters eventSearchParameters)
        {
            var queryable = _webDbContext.Events.AsQueryable().OrderBy(o => o.DateStart);

            return await PagedList<Event>.CreateAsync(queryable, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);
        }
        public async Task<IReadOnlyList<Event>> GetManyAsync()
        {
            return await _webDbContext.Events.ToListAsync();
        }
        public async Task CreateAsync(Event eventObject)
        {
            _webDbContext.Events.Add(eventObject);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(Event eventObject)
        {
            _webDbContext.Events.Update(eventObject);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync (Event eventObject)
        {
            _webDbContext.Events.Remove(eventObject);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
