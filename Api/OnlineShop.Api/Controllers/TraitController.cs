using Microsoft.AspNetCore.Mvc;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;
using OnlineShop.BLL.Services.Interfaces;

namespace OnlineShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraitController : ControllerBase
    {
        private readonly ITraitService _traitService;
        private readonly ILogger<TraitController> _logger;

        public TraitController(ITraitService traitService, ILogger<TraitController> logger)
        {
            _traitService = traitService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Getting all traits.");
            var traits = await _traitService.GetAllAsync();
            _logger.LogInformation($"Retrieved {traits.Count()} traits.");
            return Ok(traits);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Getting trait by ID: {id}");
            try
            {
                var trait = await _traitService.GetByIdAsync(id);
                _logger.LogInformation($"Trait with ID {id} retrieved successfully.");
                return Ok(trait);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Trait with ID {id} not found.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving trait with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateTraitDto model)
        {
            _logger.LogInformation("Adding new trait.");
            try
            {
                var trait = await _traitService.AddAsync(model);
                _logger.LogInformation($"Trait with ID {trait} added successfully.");
                return Ok(trait);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Failed to add trait: {ex.Message}");
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding trait.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTraitDto model)
        {
            _logger.LogInformation($"Updating trait with ID {model.Id}.");
            try
            {
                var trait = await _traitService.UpdateAsync(model);
                _logger.LogInformation($"Trait with ID {model.Id} updated successfully.");
                return Ok(trait);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Trait with ID {model.Id} not found for update.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating trait with ID {model.Id}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting trait with ID {id}.");
            try
            {
                await _traitService.DeleteAsync(id);
                _logger.LogInformation($"Trait with ID {id} deleted successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting trait with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
