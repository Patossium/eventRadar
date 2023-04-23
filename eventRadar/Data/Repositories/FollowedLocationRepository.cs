using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
   public interface IFollowedLocationRepository
    {
        Task CreateAsync(FollowedLocation followedLocation);
        Task UpdateAsync(FollowedLocation followedLocation);
        Task DeleteAsync(FollowedLocation followedLocation);
        Task<FollowedLocation?> GetAsync(int followedLocationId);
        Task<IReadOnlyList<FollowedLocation>> GetManyAsync();
    }
    public class FollowedLocationRepository : IFollowedLocationRepository
    {
        private readonly WebDbContext _webDbContext;
        public FollowedLocationRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<FollowedLocation?> GetAsync(int followedLocationId)
        {
            return await _webDbContext.FollowedLocations.FirstOrDefaultAsync(o => o.Id == followedLocationId);
        }
        public async Task<IReadOnlyList<FollowedLocation>> GetManyAsync()
        {
            return await _webDbContext.FollowedLocations.ToListAsync();
        }
        public async Task CreateAsync(FollowedLocation followedLocation)
        {
            _webDbContext.FollowedLocations.Add(followedLocation);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(FollowedLocation followedLocation)
        {
            _webDbContext.FollowedLocations.Update(followedLocation);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(FollowedLocation followedLocation)
        {
            _webDbContext.FollowedLocations.Remove(followedLocation);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
