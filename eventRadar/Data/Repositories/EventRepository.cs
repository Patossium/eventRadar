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
        Task<PagedList<Event>> GetManyPagedAsync(EventSearchParameters eventSearchParameters);
        Task<PagedList<Event>> GetManyUpcomingPagedAsync(EventSearchParameters eventSearchParameters);
        Task<PagedList<Event>> GetManyPastPagedAsync(EventSearchParameters eventSearchParameters);
        Task UpdateAsync(Event eventObject);
        Task<PagedList<Event>> GetManyFilteredAsync(string Category, EventSearchParameters eventSearchParameters);
        Task<PagedList<Event>> GetManyUpcomingFilteredAsync(string Category, EventSearchParameters eventSearchParameters);
        Task<PagedList<Event>> GetManyPastFilteredAsync(string Category, EventSearchParameters eventSearchParameters);
        Task<PagedList<Event>> GetManySearchedAsync(string search, EventSearchParameters eventSearchParameters);
        Task<PagedList<Event>> GetManyUpcomingSearchedAsync(string search, EventSearchParameters eventSearchParameters);
        Task<PagedList<Event>> GetManyPastSearchedAsync(string search, EventSearchParameters eventSearchParameters);
        Task<PagedList<Event>> GetManyFilteredSearchAsync(string search, string Category, EventSearchParameters eventSearchParameters);
        Task<PagedList<Event>> GetManyUpcomingFilteredSearchAsync(string search, string Category, EventSearchParameters eventSearchParameters);
        Task<PagedList<Event>> GetManyPastFilteredSearchAsync(string search, string Category, EventSearchParameters eventSearchParameters);
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
        public async Task<PagedList<Event>> GetManyPagedAsync(EventSearchParameters eventSearchParameters)
        {
            var queryable = _webDbContext.Events.AsQueryable().OrderBy(o => o.DateStart);

            return await PagedList<Event>.CreateAsync(queryable, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);
        }
        public async Task<PagedList<Event>> GetManyPastPagedAsync(EventSearchParameters eventSearchParameters)
        {
            var today = DateTime.Today;
            var queryable = _webDbContext.Events
                .Where(e => e.DateStart < today)
                .OrderBy(o => o.DateStart)
                .AsQueryable();

            return await PagedList<Event>.CreateAsync(queryable, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);
        }
        public async Task<PagedList<Event>> GetManyUpcomingPagedAsync(EventSearchParameters eventSearchParameters)
        {
            var today = DateTime.Today;
            var queryable = _webDbContext.Events
                .Where(e => e.DateStart >= today)
                .OrderBy(o => o.DateStart)
                .AsQueryable();

            return await PagedList<Event>.CreateAsync(queryable, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);
        }
        public async Task<PagedList<Event>> GetManyFilteredAsync(string Category, EventSearchParameters eventSearchParameters)
        {
            var filteredEvents = _webDbContext.Events.AsQueryable().Where(o => o.Category == Category).OrderBy(o => o.DateStart);

            return await PagedList<Event>.CreateAsync(filteredEvents, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);

        }
        public async Task<PagedList<Event>> GetManyPastFilteredAsync(string Category, EventSearchParameters eventSearchParameters)
        {
            var today = DateTime.Today;
            var filteredEvents = _webDbContext.Events
                .Where(e => e.Category == Category && e.DateStart < today)
                .OrderBy(e => e.DateStart)
                .AsQueryable();

            return await PagedList<Event>.CreateAsync(filteredEvents, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);

        }
        public async Task<PagedList<Event>> GetManyUpcomingFilteredAsync(string Category, EventSearchParameters eventSearchParameters)
        {
            var today = DateTime.Today;
            var filteredEvents = _webDbContext.Events
                .Where(e => e.Category == Category && e.DateStart > today)
                .OrderBy(e => e.DateStart)
                .AsQueryable();

            return await PagedList<Event>.CreateAsync(filteredEvents, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);

        }
        public async Task<PagedList<Event>> GetManyFilteredSearchAsync(string search, string Category, EventSearchParameters eventSearchParameters)
        {
            var filteredEvents = _webDbContext.Events.AsQueryable().Where(o => o.Category == Category && (o.Title.Contains(search) || o.Location.Contains(search))).OrderBy(o => o.DateStart);

            return await PagedList<Event>.CreateAsync(filteredEvents, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);

        }
        public async Task<PagedList<Event>> GetManyPastFilteredSearchAsync(string search, string Category, EventSearchParameters eventSearchParameters)
        {
            var today = DateTime.Today;
            var filteredEvents = _webDbContext.Events
                .Where(e => e.Category == Category && (e.Title.Contains(search) || e.Location.Contains(search)) && e.DateStart < today)
                .OrderBy(e => e.DateStart)
                .AsQueryable();

            return await PagedList<Event>.CreateAsync(filteredEvents, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);

        }
        public async Task<PagedList<Event>> GetManyUpcomingFilteredSearchAsync(string search, string Category, EventSearchParameters eventSearchParameters)
        {
            var today = DateTime.Today;
            var filteredEvents = _webDbContext.Events
                .Where(e => e.Category == Category && (e.Title.Contains(search) || e.Location.Contains(search)) && e.DateStart >= today)
                .OrderBy(e => e.DateStart)
                .AsQueryable();

            return await PagedList<Event>.CreateAsync(filteredEvents, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);

        }
        public async Task<PagedList<Event>> GetManySearchedAsync(string search, EventSearchParameters eventSearchParameters)
        {
            var searchedEvents = _webDbContext.Events.AsQueryable().Where(o => o.Title.Contains(search) || o.Location.Contains(search)).OrderBy(o => o.DateStart);

            return await PagedList<Event>.CreateAsync(searchedEvents, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);
        }
        public async Task<PagedList<Event>> GetManyPastSearchedAsync(string search, EventSearchParameters eventSearchParameters)
        {
            var today = DateTime.Today;
            var searchedEvents = _webDbContext.Events
                .Where(e => (e.Title.Contains(search) || e.Location.Contains(search)) && e.DateStart < today)
                .OrderBy(e => e.DateStart)
                .AsQueryable();

            return await PagedList<Event>.CreateAsync(searchedEvents, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);
        }
        public async Task<PagedList<Event>> GetManyUpcomingSearchedAsync(string search, EventSearchParameters eventSearchParameters)
        {
            var today = DateTime.Today;
            var searchedEvents = _webDbContext.Events
                .Where(e => (e.Title.Contains(search) || e.Location.Contains(search)) && e.DateStart >= today)
                .OrderBy(e => e.DateStart)
                .AsQueryable();

            return await PagedList<Event>.CreateAsync(searchedEvents, eventSearchParameters.PageNumber, eventSearchParameters.PageSize);
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
