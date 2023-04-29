using eventRadar.Controllers;
using eventRadar.Models;
using eventRadar;
using Microsoft.EntityFrameworkCore;

namespace eventRadar.Data.Repositories
{
    public interface ICityRepository
    {
        Task CreateAsync(City city);
        Task DeleteAsync(City city);
        Task<City?> GetAsync(int cityId);
        Task<IReadOnlyList<City>> GetManyAsync();
    }
    public class CityRepository : ICityRepository
    {
        private readonly WebDbContext _webDbContext;
        public CityRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
        }
        public async Task<City?> GetAsync(int cityId)
        {
            return await _webDbContext.Cities.FirstOrDefaultAsync(o => o.Id == cityId);
        }
        public async Task<IReadOnlyList<City>> GetManyAsync()
        {
            return await _webDbContext.Cities.ToListAsync();
        }
        public async Task CreateAsync(City city)
        {
            _webDbContext.Cities.Add(city);
            await _webDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(City city)
        {
            _webDbContext.Cities.Remove(city);
            await _webDbContext.SaveChangesAsync();
        }
    }
}
