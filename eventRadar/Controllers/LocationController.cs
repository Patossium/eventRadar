using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Auth.Model;
using System.Runtime.InteropServices;

namespace eventRadar.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;

        public LocationController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<LocationDto>> GetMany()
        {
            var locations = await _locationRepository.GetManyAsync();
            return locations.Select(o => new LocationDto(o.Id, o.Name, o.City));
        }
        [HttpGet()]
        [Route("{locationId}", Name = "GetLocation")]
        public async Task<ActionResult<LocationDto>> Get(int locationId)
        {
            var location = await _locationRepository.GetAsync(locationId);

            if(location == null)
            {
                return NotFound();
            }
            return new LocationDto(location.Id, location.Name, location.City);
        }

        [HttpPost]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<LocationDto>> Create(CreateLocationDto createLocationDto)
        {
            var location = new Location { Name = createLocationDto.Name, City = createLocationDto.City };
            await _locationRepository.CreateAsync(location);

            return Created("", new LocationDto(location.Id, location.Name, location.City));
        }

        [HttpPut]
        [Route("{locationId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<LocationDto>> Update(int locationId, UpdateLocationDto updateLocationDto)
        {
            var location = await _locationRepository.GetAsync(locationId);

            if(location == null)
            {
                return NotFound();
            }

            location.Name = updateLocationDto.Name;
            location.City = updateLocationDto.City;

            await _locationRepository.UpdateAsync(location);

            return Ok(new LocationDto(locationId, location.Name, location.City));
        }

        [HttpDelete]
        [Route("{locationId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult> Remove(int locationId)
        {
            var location = await _locationRepository.GetAsync(locationId);

            if(location == null)
            {
                return NotFound();
            }

            await _locationRepository.DeleteAsync(location);

            return NoContent();
        }
    }
}
