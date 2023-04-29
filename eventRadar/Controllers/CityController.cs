using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Auth.Model;

namespace eventRadar.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CityController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;
        public CityController(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }
        [HttpGet]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<IEnumerable<CityDto>> GetMany()
        {
            var cities = await _cityRepository.GetManyAsync();
            return cities.Select(o => new CityDto(o.Id, o.Name));
        }

        [HttpGet()]
        [Route("{cityId}", Name = "GetCity")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<CityDto>> Get(int cityId)
        {
            var city = await _cityRepository.GetAsync(cityId);
            if (city == null)
                return NotFound();

            return new CityDto(cityId, city.Name);
        }

        [HttpPost]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<CityDto>> Create(CreateCityDto createCityDto)
        {
            var city = new City { Name = createCityDto.Name };

            await _cityRepository.CreateAsync(city);

            return Created("", new CityDto(city.Id, city.Name));
        }

        [HttpDelete]
        [Route("{cityId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult> Remove(int cityId)
        {
            var city = await _cityRepository.GetAsync(cityId);

            if(city == null) 
                return NotFound();

            await _cityRepository.DeleteAsync(city);

            return NoContent();
        }
    }
}
