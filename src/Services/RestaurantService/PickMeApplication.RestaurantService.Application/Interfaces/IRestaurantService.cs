using PickMeApplication.RestaurantService.Application.DTOs;
using PickMeApplication.RestaurantService.Domain.Enums;

namespace PickMeApplication.RestaurantService.Application.Interfaces;

public interface IRestaurantService
{
    // Restaurant CRUD operations
    Task<RestaurantDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RestaurantDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RestaurantCreatedResponse> CreateAsync(CreateRestaurantDto dto, CancellationToken cancellationToken = default);
    Task<RestaurantDto?> UpdateAsync(Guid id, UpdateRestaurantDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Restaurant management
    Task<IEnumerable<RestaurantDto>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<RestaurantStatusResponse> UpdateStatusAsync(Guid id, RestaurantStatus status, string? reason = null, CancellationToken cancellationToken = default);
    Task<RestaurantOrderAcceptanceResponse> ToggleOrderAcceptanceAsync(Guid id, string? reason = null, CancellationToken cancellationToken = default);
    Task<RestaurantDto?> UpdateLocationAsync(Guid id, UpdateRestaurantLocationDto dto, CancellationToken cancellationToken = default);
    
    // Search and discovery
    Task<IEnumerable<RestaurantSummaryDto>> SearchRestaurantsAsync(RestaurantSearchDto searchDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<RestaurantSummaryDto>> GetRestaurantsNearLocationAsync(double latitude, double longitude, double radiusInKm = 10.0, CancellationToken cancellationToken = default);
    Task<IEnumerable<RestaurantSummaryDto>> GetActiveRestaurantsAsync(CancellationToken cancellationToken = default);
    
    // Business hours management
    Task<RestaurantDto?> UpdateBusinessHoursAsync(Guid restaurantId, IEnumerable<CreateBusinessHoursDto> businessHoursDtos, CancellationToken cancellationToken = default);
    Task<bool> IsRestaurantOpenAsync(Guid restaurantId, DateTime? dateTime = null, CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> RestaurantExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> OwnerCanManageRestaurantAsync(Guid restaurantId, Guid ownerId, CancellationToken cancellationToken = default);
}