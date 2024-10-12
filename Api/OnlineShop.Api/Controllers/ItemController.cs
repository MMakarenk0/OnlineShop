using Microsoft.AspNetCore.Mvc;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;
using OnlineShop.BLL.Services.Interfaces;

namespace OnlineShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IItemService itemService, ILogger<ItemController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Getting all items.");
            var items = await _itemService.GetAllAsync();
            _logger.LogInformation($"Retrieved {items.Count()} items.");
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Getting item by ID: {id}");
            try
            {
                var item = await _itemService.GetByIdAsync(id);
                _logger.LogInformation($"Item with ID {id} retrieved successfully.");
                return Ok(item);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Item with ID {id} not found.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving item with ID {id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetByFilters([FromQuery] ItemFilterDto filters)
        {
            _logger.LogInformation($"Getting items by filters: {filters}");
            try
            {
                var filteredItems = await _itemService.GetByFilters(filters);
                _logger.LogInformation($"Filtered items retrieved successfully.");
                return Ok(filteredItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error filtering items with filters: {filters}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateItemDto model)
        {
            _logger.LogInformation("Adding new item.");
            try
            {
                var item = await _itemService.AddAsync(model);
                _logger.LogInformation($"Item with ID {item} added successfully.");
                return Ok(item);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Failed to add item: {ex.Message}");
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateItemDto model)
        {
            _logger.LogInformation($"Updating item with ID {model.Id}.");
            try
            {
                var item = await _itemService.UpdateAsync(model);
                _logger.LogInformation($"Item with ID {model.Id} updated successfully.");
                return Ok(item);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Item with ID {model.Id} not found for update.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating item with ID {model.Id}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting item with ID {id}.");
            try
            {
                await _itemService.DeleteAsync(id);
                _logger.LogInformation($"Item with ID {id} deleted successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting item with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
