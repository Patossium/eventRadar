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
    [Route("api/websites")]
    public class WebsiteController : ControllerBase
    {
        private readonly IWebsiteRepository _websiteRepository;

        public WebsiteController(IWebsiteRepository websiteRepository)
        {
            _websiteRepository = websiteRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<WebsiteDto>> GetMany()
        {
            var website = await _websiteRepository.GetManyAsync();
            return website.Select(o => new WebsiteDto(o.Id, o.Url, o.TitlePath, o.LocationPath, o.PricePath, o.DatePath, o.ImagePath, o.TicketPath));
        }

        [HttpGet()]
        [Route("{websiteId}", Name = "GetWebsite")]
        public async Task<ActionResult<WebsiteDto>> Get(string websiteId)
        {
            var website = await _websiteRepository.GetAsync(websiteId);
            if(website == null)
            {
                return NotFound();
            }

            return new WebsiteDto(website.Id, website.Url, website.TitlePath, website.LocationPath, website.PricePath, website.DatePath, website.ImagePath, website.TicketPath);
        }

        [HttpPost]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<WebsiteDto>> Create(CreateWebsiteDto createWebsiteDto)
        {
            var website = new Website { Url = createWebsiteDto.Url, TitlePath = createWebsiteDto.TitlePath, LocationPath = createWebsiteDto.LocationPath, 
                PricePath = createWebsiteDto.PricePath, DatePath = createWebsiteDto.DatePath, ImagePath = createWebsiteDto.ImagePath, TicketPath = createWebsiteDto.TicketPath };

            await _websiteRepository.CreateAsync(website);

            return Created("", new WebsiteDto(website.Id, website.Url, website.TitlePath, website.LocationPath, website.PricePath, website.DatePath, website.ImagePath, website.TicketPath));

        }

        [HttpPut]
        [Route("{websiteId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<WebsiteDto>> Update(string websiteId, UpdateWebsiteDto updateWebsiteDto)
        {
            var website = await _websiteRepository.GetAsync(websiteId);

            if(website == null)
            {
                return NotFound();
            }

            website.Url = updateWebsiteDto.Url;
            website.TitlePath = updateWebsiteDto.TitlePath;
            website.LocationPath = updateWebsiteDto.LocationPath;
            website.PricePath = updateWebsiteDto.PricePath;
            website.DatePath = updateWebsiteDto.DatePath;
            website.ImagePath = updateWebsiteDto.ImagePath;
            website.TicketPath = updateWebsiteDto.TicketPath;

            await _websiteRepository.UpdateAsync(website);

            return Ok(new WebsiteDto (websiteId, website.Url, website.TitlePath, website.LocationPath, website.PricePath, website.DatePath, website.ImagePath, website.TicketPath));
        }

        [HttpDelete]
        [Route("{websiteId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult> Remove(string websiteId)
        {
            var website = await _websiteRepository.GetAsync(websiteId);

            if (website == null)
            { 
                return NotFound(); 
            }

            await _websiteRepository.DeleteAsync(website);

            return NoContent();
        }
    }
}
