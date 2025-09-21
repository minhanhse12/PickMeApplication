using Microsoft.EntityFrameworkCore;
using PickMeApplication.RestaurantService.Domain.Entities;
using PickMeApplication.RestaurantService.Domain.Enums;
using PickMeApplication.RestaurantService.Domain.Repositories;
using PickMeApplication.RestaurantService.Infrastructure.Data;

namespace PickMeApplication.RestaurantService.Infrastructure.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly RestaurantDbContext _context;

    public RestaurantRepository(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants
            .Include(r => r.Location)
            .Include(r => r.BusinessHours)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Restaurant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants
            .Include(r => r.Location)
            .Include(r => r.BusinessHours)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        _context.Restaurants.Update(restaurant);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var restaurant = await _context.Restaurants.FindAsync(new object[] { id }, cancellationToken);
        if (restaurant != null)
        {
            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Restaurant>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants
            .Include(r => r.Location)
            .Include(r => r.BusinessHours)
            .Where(r => r.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Restaurant>> GetByCuisineTypeAsync(CuisineType cuisineType, CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants
            .Include(r => r.Location)
            .Include(r => r.BusinessHours)
            .Where(r => r.CuisineType == cuisineType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Restaurant>> GetActiveRestaurantsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants
            .Include(r => r.Location)
            .Include(r => r.BusinessHours)
            .Where(r => r.Status == RestaurantStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Restaurant>> GetRestaurantsAcceptingOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants
            .Include(r => r.Location)
            .Include(r => r.BusinessHours)
            .Where(r => r.Status == RestaurantStatus.Active && r.IsAcceptingOrders)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Restaurant>> GetRestaurantsNearLocationAsync(
        double latitude, 
        double longitude, 
        double radiusInKm, 
        CancellationToken cancellationToken = default)
    {
        // Using Haversine formula for distance calculation in SQL
        // This is an approximation for MySQL - for production, consider using spatial functions
        var restaurants = await _context.Restaurants
            .Include(r => r.Location)
            .Include(r => r.BusinessHours)
            .Where(r => r.Status == RestaurantStatus.Active)
            .ToListAsync(cancellationToken);

        // Filter by distance in memory (for simplicity)
        // In production, this should be done in the database with proper spatial functions
        return restaurants.Where(r => 
            r.Location.CalculateDistanceTo(latitude, longitude) <= radiusInKm);
    }

    public async Task<IEnumerable<Restaurant>> GetRestaurantsWithinDeliveryRangeAsync(
        double latitude, 
        double longitude, 
        CancellationToken cancellationToken = default)
    {
        var restaurants = await _context.Restaurants
            .Include(r => r.Location)
            .Include(r => r.BusinessHours)
            .Where(r => r.Status == RestaurantStatus.Active && r.IsAcceptingOrders)
            .ToListAsync(cancellationToken);

        // Filter by each restaurant's delivery range
        return restaurants.Where(r => 
            r.Location.CalculateDistanceTo(latitude, longitude) <= r.MaxDeliveryDistanceKm);
    }

    public async Task<IEnumerable<Restaurant>> SearchRestaurantsAsync(
        string? searchTerm = null, 
        CuisineType? cuisineType = null, 
        bool? isAcceptingOrders = null, 
        double? latitude = null, 
        double? longitude = null, 
        double? radiusInKm = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Restaurants
            .Include(r => r.Location)
            .Include(r => r.BusinessHours)
            .Where(r => r.Status == RestaurantStatus.Active);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(r => 
                r.Name.Contains(searchTerm) || 
                r.Description.Contains(searchTerm) ||
                r.Location.Address.Contains(searchTerm));
        }

        if (cuisineType.HasValue)
        {
            query = query.Where(r => r.CuisineType == cuisineType.Value);
        }

        if (isAcceptingOrders.HasValue)
        {
            query = query.Where(r => r.IsAcceptingOrders == isAcceptingOrders.Value);
        }

        var restaurants = await query.ToListAsync(cancellationToken);

        // Apply location filter if provided
        if (latitude.HasValue && longitude.HasValue && radiusInKm.HasValue)
        {
            restaurants = restaurants.Where(r => 
                r.Location.CalculateDistanceTo(latitude.Value, longitude.Value) <= radiusInKm.Value)
                .ToList();
        }

        return restaurants;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants.AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<bool> OwnerHasRestaurantAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants.AnyAsync(r => r.OwnerId == ownerId, cancellationToken);
    }
}