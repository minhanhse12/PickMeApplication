using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickMeApplication.RestaurantService.Application.DTOs;
using PickMeApplication.RestaurantService.Application.Interfaces;
using PickMeApplication.RestaurantService.Domain.Enums;

namespace PickMeApplication.RestaurantService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantsController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    /// <summary>
    /// Get all restaurants
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RestaurantSummaryDto>>> GetAllRestaurants()
    {
        var restaurants = await _restaurantService.GetActiveRestaurantsAsync();
        return Ok(restaurants);
    }

    /// <summary>
    /// Get restaurant by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RestaurantDto>> GetRestaurant(Guid id)
    {
        var restaurant = await _restaurantService.GetByIdAsync(id);
        if (restaurant == null)
            return NotFound($"Restaurant with ID {id} not found");

        return Ok(restaurant);
    }

    /// <summary>
    /// Create new restaurant
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "RestaurantOwner")]
    public async Task<ActionResult<RestaurantCreatedResponse>> CreateRestaurant([FromBody] CreateRestaurantDto dto)
    {
        try
        {
            var response = await _restaurantService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetRestaurant), new { id = response.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update restaurant information
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<ActionResult<RestaurantDto>> UpdateRestaurant(Guid id, [FromBody] UpdateRestaurantDto dto)
    {
        try
        {
            var restaurant = await _restaurantService.UpdateAsync(id, dto);
            if (restaurant == null)
                return NotFound($"Restaurant with ID {id} not found");

            return Ok(restaurant);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete restaurant
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteRestaurant(Guid id)
    {
        var deleted = await _restaurantService.DeleteAsync(id);
        if (!deleted)
            return NotFound($"Restaurant with ID {id} not found");

        return NoContent();
    }

    /// <summary>
    /// Get restaurants by owner ID
    /// </summary>
    [HttpGet("owner/{ownerId:guid}")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetRestaurantsByOwner(Guid ownerId)
    {
        var restaurants = await _restaurantService.GetByOwnerIdAsync(ownerId);
        return Ok(restaurants);
    }

    /// <summary>
    /// Update restaurant status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RestaurantStatusResponse>> UpdateRestaurantStatus(
        Guid id, 
        [FromBody] UpdateRestaurantStatusRequest request)
    {
        try
        {
            var response = await _restaurantService.UpdateStatusAsync(id, request.Status, request.Reason);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Toggle restaurant order acceptance
    /// </summary>
    [HttpPatch("{id:guid}/order-acceptance")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<ActionResult<RestaurantOrderAcceptanceResponse>> ToggleOrderAcceptance(
        Guid id, 
        [FromBody] ToggleOrderAcceptanceRequest request)
    {
        try
        {
            var response = await _restaurantService.ToggleOrderAcceptanceAsync(id, request.Reason);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update restaurant location
    /// </summary>
    [HttpPut("{id:guid}/location")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<ActionResult<RestaurantDto>> UpdateRestaurantLocation(
        Guid id, 
        [FromBody] UpdateRestaurantLocationDto dto)
    {
        try
        {
            var restaurant = await _restaurantService.UpdateLocationAsync(id, dto);
            if (restaurant == null)
                return NotFound($"Restaurant with ID {id} not found");

            return Ok(restaurant);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Search restaurants with filters
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<RestaurantSummaryDto>>> SearchRestaurants([FromBody] RestaurantSearchDto searchDto)
    {
        var restaurants = await _restaurantService.SearchRestaurantsAsync(searchDto);
        return Ok(restaurants);
    }

    /// <summary>
    /// Get restaurants near location
    /// </summary>
    [HttpGet("near")]
    public async Task<ActionResult<IEnumerable<RestaurantSummaryDto>>> GetRestaurantsNearLocation(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusInKm = 10.0)
    {
        var restaurants = await _restaurantService.GetRestaurantsNearLocationAsync(latitude, longitude, radiusInKm);
        return Ok(restaurants);
    }

    /// <summary>
    /// Update restaurant business hours
    /// </summary>
    [HttpPut("{id:guid}/business-hours")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<ActionResult<RestaurantDto>> UpdateBusinessHours(
        Guid id, 
        [FromBody] List<CreateBusinessHoursDto> businessHours)
    {
        try
        {
            var restaurant = await _restaurantService.UpdateBusinessHoursAsync(id, businessHours);
            if (restaurant == null)
                return NotFound($"Restaurant with ID {id} not found");

            return Ok(restaurant);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Check if restaurant is currently open
    /// </summary>
    [HttpGet("{id:guid}/is-open")]
    public async Task<ActionResult<RestaurantOpenStatusResponse>> IsRestaurantOpen(Guid id)
    {
        var exists = await _restaurantService.RestaurantExistsAsync(id);
        if (!exists)
            return NotFound($"Restaurant with ID {id} not found");

        var isOpen = await _restaurantService.IsRestaurantOpenAsync(id);
        return Ok(new RestaurantOpenStatusResponse(id, isOpen, DateTime.Now));
    }
}

// Request DTOs for API endpoints
public record UpdateRestaurantStatusRequest(RestaurantStatus Status, string? Reason = null);
public record ToggleOrderAcceptanceRequest(string? Reason = null);
public record RestaurantOpenStatusResponse(Guid RestaurantId, bool IsOpen, DateTime CheckedAt);