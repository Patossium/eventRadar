using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface ILocationRepository
    {
        Task CreateAsync(Location location);
        Task DeleteAsync(Location location);
        Task UpdateAsync(Location location);
        Task<Location?> GetAsync(string locationId);
        Task<IReadOnlyList<Location>> GetManyAsync();
    }
    public class LocationRepository : ILocationRepository
    {
        private readonly WebDbContext _webDbContext;
        public LocationRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<Location?> GetAsync(string locationId)
        {
            return await _webDbContext.Locations.FirstOrDefaultAsync(o => o.Id == locationId);
        }
        public async Task<IReadOnlyList<Location>> GetManyAsync()
        {
            return await _webDbContext.Locations.ToListAsync();
        }
        public async Task CreateAsync(Location location)
        {
            _webDbContext.Locations.Add(location);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(Location location)
        {
            _webDbContext.Locations.Update(location);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Location location)
        {
            _webDbContext.Locations.Remove(location);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
