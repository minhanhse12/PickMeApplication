using PickMeApplication.RestaurantService.Domain.Entities;

namespace PickMeApplication.RestaurantService.Domain.Repositories;

public interface IBusinessHoursRepository
{
    // Basic CRUD operations
    Task<BusinessHours?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BusinessHours>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(BusinessHours businessHours, CancellationToken cancellationToken = default);
    Task UpdateAsync(BusinessHours businessHours, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Restaurant specific queries
    Task<IEnumerable<BusinessHours>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    Task<BusinessHours?> GetRestaurantBusinessHoursForDayAsync(Guid restaurantId, int dayOfWeek, CancellationToken cancellationToken = default);
    
    // Batch operations
    Task AddRangeAsync(IEnumerable<BusinessHours> businessHours, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<BusinessHours> businessHours, CancellationToken cancellationToken = default);
    Task DeleteByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    
    // Business logic queries
    Task<IEnumerable<BusinessHours>> GetOpenRestaurantsAtTimeAsync(DateTime dateTime, CancellationToken cancellationToken = default);
    Task<bool> IsRestaurantOpenAtAsync(Guid restaurantId, DateTime dateTime, CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> RestaurantHasBusinessHoursForDayAsync(Guid restaurantId, int dayOfWeek, CancellationToken cancellationToken = default);
}