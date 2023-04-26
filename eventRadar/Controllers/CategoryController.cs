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
    [Route("api/Category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryDto>> GetMany()
        {
            var category = await _categoryRepository.GetManyAsync();
            return category.Select(o => new CategoryDto(o.Id, o.Name, o.WebsiteName));
        }

        [HttpGet()]
        [Route("{cateogryId}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDto>> Get(int categoryId)
        {
            var category = await _categoryRepository.GetAsync(categoryId);
            if(category == null)
            {
                return NotFound();
            }
            return new CategoryDto(category.Id, category.Name, category.WebsiteName);
        }

        [HttpPost]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto createCategoryDto)
        {
            var category = new Category { Name = createCategoryDto.Name, WebsiteName = createCategoryDto.WebsiteName };

            await _categoryRepository.CreateAsync(category);

            return Created("", new CategoryDto(category.Id, category.Name, category.WebsiteName));
        }

        [HttpPut]
        [Route("{categoryId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<CategoryDto>> Update(int categoryId, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryRepository.GetAsync(categoryId);

            if(category == null)
            {
                return NotFound();
            }

            category.Name = updateCategoryDto.Name;
            category.WebsiteName = updateCategoryDto.WebsiteName;

            await _categoryRepository.UpdateAsync(category);

            return Ok(new CategoryDto(categoryId, category.Name, category.WebsiteName));
        }

        [HttpDelete]
        [Route("{categoryId}")]
        [Authorize(Roles =SystemRoles.Administrator)]
        public async Task<ActionResult> Remove(string categoryId)
        {
            var category = await _categoryRepository.GetAsync(categoryId);

            if(category == null)
                return NotFound();

            await _categoryRepository.DeleteAsync(category);

            return NoContent();
        }
    }
}
