using PickMeApplication.RestaurantService.Domain.Entities;
using PickMeApplication.RestaurantService.Domain.Enums;

namespace PickMeApplication.RestaurantService.Domain.Repositories;

public interface IRestaurantRepository
{
    // Basic CRUD operations
    Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Restaurant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Restaurant restaurant, CancellationToken cancellationToken = default);
    Task UpdateAsync(Restaurant restaurant, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Business specific queries
    Task<IEnumerable<Restaurant>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Restaurant>> GetByCuisineTypeAsync(CuisineType cuisineType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Restaurant>> GetActiveRestaurantsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Restaurant>> GetRestaurantsAcceptingOrdersAsync(CancellationToken cancellationToken = default);
    
    // Location-based queries
    Task<IEnumerable<Restaurant>> GetRestaurantsNearLocationAsync(
        double latitude, 
        double longitude, 
        double radiusInKm,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Restaurant>> GetRestaurantsWithinDeliveryRangeAsync(
        double latitude, 
        double longitude,
        CancellationToken cancellationToken = default);
    
    // Search and filtering
    Task<IEnumerable<Restaurant>> SearchRestaurantsAsync(
        string? searchTerm = null,
        CuisineType? cuisineType = null,
        bool? isAcceptingOrders = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusInKm = null,
        CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> OwnerHasRestaurantAsync(Guid ownerId, CancellationToken cancellationToken = default);
}