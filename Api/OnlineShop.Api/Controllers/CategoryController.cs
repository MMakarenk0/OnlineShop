using Microsoft.AspNetCore.Mvc;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Update;
using OnlineShop.BLL.Services.Interfaces;

namespace OnlineShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Getting all categories.");
            var categories = await _categoryService.GetAllAsync();
            _logger.LogInformation($"Successfully retrieved {categories.Count()} categories.");
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("Getting category by ID: {CategoryId}", id);

            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found.");
                    return NotFound();
                }

                _logger.LogInformation($"Successfully retrieved category with ID {id}.");
                return Ok(category);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Category with ID {id} was not found.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the category with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateCategoryDto model)
        {
            _logger.LogInformation("Adding a new category.");

            try
            {
                var category = await _categoryService.AddAsync(model);
                _logger.LogInformation($"Category with ID {category} successfully added.");
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Failed to add category due to missing key.");
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the category.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateCategoryDto model)
        {
            _logger.LogInformation($"Updating category with ID {model.Id}.");

            try
            {
                var category = await _categoryService.UpdateAsync(model);
                _logger.LogInformation($"Category with ID {model.Id} successfully updated.");
                return Ok(category);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Category with ID {model.Id} not found for update.");
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Failed to update category with ID {model.Id}.");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the category with ID {model.Id}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting category with ID {id}.");

            try
            {
                await _categoryService.DeleteAsync(id);
                _logger.LogInformation($"Category with ID {id} successfully deleted.");
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Category with ID {id} not found for deletion.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the category with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
