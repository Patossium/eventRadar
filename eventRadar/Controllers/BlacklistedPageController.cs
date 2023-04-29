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
    [Route("api/blacklistedPages")]
    public class BlacklistedPageController : ControllerBase
    {
        private readonly IBlacklistedPageRepository _blacklistedRepository;

        public BlacklistedPageController(IBlacklistedPageRepository blacklistedRepository)
        {
            _blacklistedRepository = blacklistedRepository;
        }

        [HttpGet]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<IEnumerable<BlacklistedPageDto>> GetMany()
        {
            var blacklistedPage = await _blacklistedRepository.GetManyAsync();
            return blacklistedPage.Select(o => new BlacklistedPageDto(o.Id, o.Url, o.Comment));
        }

        [HttpGet()]
        [Route("{blacklistedPageId}", Name = "GetBlacklistedPage")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<BlacklistedPageDto>> Get(int blacklistedPageId)
        {
            var blacklistedPage = await _blacklistedRepository.GetAsync(blacklistedPageId);
            if(blacklistedPage == null)
            {
                return NotFound();
            }
            return new BlacklistedPageDto(blacklistedPageId, blacklistedPage.Url, blacklistedPage.Comment);
        }

        [HttpPost]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<BlacklistedPageDto>> Create(CreateBlacklistedPageDto createBlacklistedPageDto)
        {
            var blacklistedPage = new BlacklistedPage { Url = createBlacklistedPageDto.Url, Comment = createBlacklistedPageDto.Comment };

            await _blacklistedRepository.CreateAsync(blacklistedPage);

            return Created("", new BlacklistedPageDto(blacklistedPage.Id, blacklistedPage.Url, blacklistedPage.Comment));
        }

        [HttpPut]
        [Route("{blacklistedId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<BlacklistedPageDto>> Updated(int blacklistedId, UpdateBlacklistedPageDto updateBlacklistedPageDto)
        {
            var blacklistedPage = await _blacklistedRepository.GetAsync(blacklistedId);

            if(blacklistedPage == null)
            {
                return NotFound();
            }

            blacklistedPage.Url = updateBlacklistedPageDto.Url;
            blacklistedPage.Comment = updateBlacklistedPageDto.Comment;

            await _blacklistedRepository.UpdateAsync(blacklistedPage);

            return Ok(new BlacklistedPageDto(blacklistedId, blacklistedPage.Url, blacklistedPage.Comment));
        }

        [HttpDelete]
        [Route("{blacklistedPageId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult> Remove(int blacklistedPageId)
        {
            var blacklistedPage = await _blacklistedRepository.GetAsync(blacklistedPageId);
            if (blacklistedPage == null)
            {
                return NotFound();
            }

            await _blacklistedRepository.DeleteAsync(blacklistedPage);

            return NoContent();
        }
    }
}
