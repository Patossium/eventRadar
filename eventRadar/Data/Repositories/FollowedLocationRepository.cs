using eventRadar.Auth.Model;
using eventRadar.Controllers;
using eventRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface IFollowedLocationRepository
    {
        Task CreateAsync(FollowedLocation followedLocation);
        Task DeleteAsync(FollowedLocation followedLocation);
        Task<FollowedLocation?> GetAsync(User user, string followedLocationId);
        Task<IReadOnlyList<FollowedLocation>> GetManyAsync(User user);
    }
    public class FollowedLocationRepository : IFollowedLocationRepository
    {
        private readonly WebDbContext _webDbContext;
        public FollowedLocationRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<FollowedLocation?> GetAsync(User user, string followedLocationId)
        {
            return await _webDbContext.FollowedLocations.Where(o => o.User == user).FirstOrDefaultAsync(o => o.Id == followedLocationId);
        }
        public async Task<IReadOnlyList<FollowedLocation>> GetManyAsync(User user)
        {
            return await _webDbContext.FollowedLocations.Where(o => o.User == user).ToListAsync();
        }
        public async Task CreateAsync(FollowedLocation followedLocation)
        {
            _webDbContext.FollowedLocations.Add(followedLocation);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(FollowedLocation followedLocation)
        {
            _webDbContext.FollowedLocations.Remove(followedLocation);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
