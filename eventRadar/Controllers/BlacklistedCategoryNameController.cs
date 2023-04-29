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
    [Route("api/blacklistedCategoryNames")]
    public class BlacklistedCategoryNameController :ControllerBase
    {
        private readonly IBlacklistedCategoryNameRepository _blacklistedCategoryRepository;
        public BlacklistedCategoryNameController(IBlacklistedCategoryNameRepository blacklistedCategoryRepository)
        {
            _blacklistedCategoryRepository = blacklistedCategoryRepository;
        }

        [HttpGet]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<IEnumerable<BlacklistedCategoryNameDto>> GetMany()
        {
            var blacklistedCategory = await _blacklistedCategoryRepository.GetManyAsync();
            return blacklistedCategory.Select(o => new BlacklistedCategoryNameDto(o.Id, o.Name));
        }

        [HttpGet()]
        [Route("{blacklistedCategoryNameId}", Name = "GetBlacklistedCategoryName")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<BlacklistedCategoryNameDto>> Get(int blacklistedCategoryNameId)
        {
            var blacklistedCategory = await _blacklistedCategoryRepository.GetAsync(blacklistedCategoryNameId);

            if(blacklistedCategory == null) 
                return NotFound();

            return new BlacklistedCategoryNameDto(blacklistedCategoryNameId, blacklistedCategory.Name);
        }

        [HttpPost]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<BlacklistedCategoryNameDto>> Create(CreateBlacklistedCategoryNameDto createBlacklistedCategoryNameDto)
        {
            var blacklistedCategory = new BlacklistedCategoryName { Name = createBlacklistedCategoryNameDto.Name };

            await _blacklistedCategoryRepository.CreateAsync(blacklistedCategory);

            return Created("", new BlacklistedCategoryNameDto(blacklistedCategory.Id, blacklistedCategory.Name));
        }

        [HttpPut]
        [Route("{blacklistedCategoryNameId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<BlacklistedCategoryNameDto>> Updated(int blacklistedCategoryNameId, UpdateBlacklistedCategoryNameDto updateBlacklistedCategoryNameDto)
        {
            var blacklistedCategory = await _blacklistedCategoryRepository.GetAsync(blacklistedCategoryNameId);

            if(blacklistedCategory == null) 
                return NotFound();

            blacklistedCategory.Name = updateBlacklistedCategoryNameDto.Name;

            await _blacklistedCategoryRepository.UpdateAsync(blacklistedCategory);

            return Ok(new BlacklistedCategoryNameDto(blacklistedCategoryNameId, blacklistedCategory.Name));
        }

        [HttpDelete]
        [Route("{blacklistedCategoryNameId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult> Remove(int blacklistedCategoryNameId)
        {
            var blacklistedCategory = await _blacklistedCategoryRepository.GetAsync(blacklistedCategoryNameId);

            if(blacklistedCategory == null)
                return NotFound();

            await _blacklistedCategoryRepository.DeleteAsync(blacklistedCategory);

            return NoContent();
        }
    }
}
