using PickMeApplication.RestaurantService.Application.DTOs;
using PickMeApplication.RestaurantService.Application.Interfaces;
using PickMeApplication.RestaurantService.Domain.Entities;
using PickMeApplication.RestaurantService.Domain.Enums;
using PickMeApplication.RestaurantService.Domain.Repositories;

namespace PickMeApplication.RestaurantService.Application.Services;

public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IBusinessHoursRepository _businessHoursRepository;

    public RestaurantService(
        IRestaurantRepository restaurantRepository,
        IBusinessHoursRepository businessHoursRepository)
    {
        _restaurantRepository = restaurantRepository;
        _businessHoursRepository = businessHoursRepository;
    }

    public async Task<RestaurantDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(id, cancellationToken);
        return restaurant == null ? null : MapToRestaurantDto(restaurant);
    }

    public async Task<IEnumerable<RestaurantDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var restaurants = await _restaurantRepository.GetAllAsync(cancellationToken);
        return restaurants.Select(MapToRestaurantDto);
    }

    public async Task<RestaurantCreatedResponse> CreateAsync(CreateRestaurantDto dto, CancellationToken cancellationToken = default)
    {
        var restaurant = new Restaurant
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            CuisineType = dto.CuisineType,
            Status = RestaurantStatus.Pending,
            OwnerId = dto.OwnerId,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            IsAcceptingOrders = false,
            DeliveryFeePerKm = dto.DeliveryFeePerKm,
            MaxDeliveryDistanceKm = dto.MaxDeliveryDistanceKm,
            MinimumOrderAmount = dto.MinimumOrderAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create location
        var location = new RestaurantLocation
        {
            Id = Guid.NewGuid(),
            RestaurantId = restaurant.Id,
            Address = dto.Address,
            City = dto.City,
            District = dto.District,
            Ward = dto.Ward,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            UpdatedAt = DateTime.UtcNow
        };

        restaurant.Location = location;

        // Create default business hours (Monday to Sunday, 9 AM - 10 PM)
        var businessHours = new List<BusinessHours>();
        for (int day = 0; day < 7; day++)
        {
            businessHours.Add(BusinessHours.CreateDefault(day, restaurant.Id));
        }

        restaurant.BusinessHours = businessHours;

        await _restaurantRepository.AddAsync(restaurant, cancellationToken);

        return new RestaurantCreatedResponse(
            restaurant.Id,
            restaurant.Name,
            "Restaurant created successfully and is pending approval"
        );
    }

    public async Task<RestaurantDto?> UpdateAsync(Guid id, UpdateRestaurantDto dto, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(id, cancellationToken);
        if (restaurant == null) return null;

        restaurant.Name = dto.Name;
        restaurant.Description = dto.Description;
        restaurant.CuisineType = dto.CuisineType;
        restaurant.PhoneNumber = dto.PhoneNumber;
        restaurant.Email = dto.Email;
        restaurant.DeliveryFeePerKm = dto.DeliveryFeePerKm;
        restaurant.MaxDeliveryDistanceKm = dto.MaxDeliveryDistanceKm;
        restaurant.MinimumOrderAmount = dto.MinimumOrderAmount;
        restaurant.UpdatedAt = DateTime.UtcNow;

        await _restaurantRepository.UpdateAsync(restaurant, cancellationToken);
        return MapToRestaurantDto(restaurant);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _restaurantRepository.ExistsAsync(id, cancellationToken);
        if (!exists) return false;

        await _restaurantRepository.DeleteAsync(id, cancellationToken);
        return true;
    }

    public async Task<IEnumerable<RestaurantDto>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        var restaurants = await _restaurantRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        return restaurants.Select(MapToRestaurantDto);
    }

    public async Task<RestaurantStatusResponse> UpdateStatusAsync(Guid id, RestaurantStatus status, string? reason = null, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(id, cancellationToken);
        if (restaurant == null)
            throw new ArgumentException($"Restaurant with ID {id} not found");

        var oldStatus = restaurant.Status;
        restaurant.UpdateStatus(status, reason);
        await _restaurantRepository.UpdateAsync(restaurant, cancellationToken);

        return new RestaurantStatusResponse(
            id,
            status,
            $"Restaurant status updated from {oldStatus} to {status}"
        );
    }

    public async Task<RestaurantOrderAcceptanceResponse> ToggleOrderAcceptanceAsync(Guid id, string? reason = null, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(id, cancellationToken);
        if (restaurant == null)
            throw new ArgumentException($"Restaurant with ID {id} not found");

        restaurant.ToggleOrderAcceptance(reason);
        await _restaurantRepository.UpdateAsync(restaurant, cancellationToken);

        return new RestaurantOrderAcceptanceResponse(
            id,
            restaurant.IsAcceptingOrders,
            restaurant.IsAcceptingOrders ? "Restaurant is now accepting orders" : "Restaurant has stopped accepting orders"
        );
    }

    public async Task<RestaurantDto?> UpdateLocationAsync(Guid id, UpdateRestaurantLocationDto dto, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(id, cancellationToken);
        if (restaurant == null) return null;

        restaurant.Location.Address = dto.Address;
        restaurant.Location.City = dto.City;
        restaurant.Location.District = dto.District;
        restaurant.Location.Ward = dto.Ward;
        restaurant.Location.Latitude = dto.Latitude;
        restaurant.Location.Longitude = dto.Longitude;
        restaurant.Location.UpdatedAt = DateTime.UtcNow;
        restaurant.UpdatedAt = DateTime.UtcNow;

        await _restaurantRepository.UpdateAsync(restaurant, cancellationToken);
        return MapToRestaurantDto(restaurant);
    }

    public async Task<IEnumerable<RestaurantSummaryDto>> SearchRestaurantsAsync(RestaurantSearchDto searchDto, CancellationToken cancellationToken = default)
    {
        var restaurants = await _restaurantRepository.SearchRestaurantsAsync(
            searchDto.SearchTerm,
            searchDto.CuisineType,
            searchDto.IsAcceptingOrders,
            searchDto.Latitude,
            searchDto.Longitude,
            searchDto.RadiusInKm,
            cancellationToken);

        var result = new List<RestaurantSummaryDto>();

        foreach (var restaurant in restaurants)
        {
            var summary = MapToRestaurantSummaryDto(restaurant);

            // Calculate distance if user location provided
            if (searchDto.Latitude.HasValue && searchDto.Longitude.HasValue)
            {
                summary = summary with 
                { 
                    DistanceKm = restaurant.Location.CalculateDistanceTo(searchDto.Latitude.Value, searchDto.Longitude.Value),
                    EstimatedDeliveryFee = restaurant.CalculateDeliveryFee(restaurant.Location.CalculateDistanceTo(searchDto.Latitude.Value, searchDto.Longitude.Value))
                };
            }

            // Check if currently open
            summary = summary with { IsCurrentlyOpen = restaurant.IsOpenAt(DateTime.Now) };

            result.Add(summary);
        }

        return result.OrderBy(r => r.DistanceKm ?? 0);
    }

    public async Task<IEnumerable<RestaurantSummaryDto>> GetRestaurantsNearLocationAsync(double latitude, double longitude, double radiusInKm = 10.0, CancellationToken cancellationToken = default)
    {
        var restaurants = await _restaurantRepository.GetRestaurantsNearLocationAsync(latitude, longitude, radiusInKm, cancellationToken);
        
        return restaurants.Select(restaurant => 
        {
            var summary = MapToRestaurantSummaryDto(restaurant);
            var distance = restaurant.Location.CalculateDistanceTo(latitude, longitude);
            
            return summary with 
            { 
                DistanceKm = distance,
                EstimatedDeliveryFee = restaurant.CalculateDeliveryFee(distance),
                IsCurrentlyOpen = restaurant.IsOpenAt(DateTime.Now)
            };
        }).OrderBy(r => r.DistanceKm);
    }

    public async Task<IEnumerable<RestaurantSummaryDto>> GetActiveRestaurantsAsync(CancellationToken cancellationToken = default)
    {
        var restaurants = await _restaurantRepository.GetActiveRestaurantsAsync(cancellationToken);
        return restaurants.Select(restaurant => 
        {
            var summary = MapToRestaurantSummaryDto(restaurant);
            return summary with { IsCurrentlyOpen = restaurant.IsOpenAt(DateTime.Now) };
        });
    }

    public async Task<RestaurantDto?> UpdateBusinessHoursAsync(Guid restaurantId, IEnumerable<CreateBusinessHoursDto> businessHoursDtos, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant == null) return null;

        // Delete existing business hours
        await _businessHoursRepository.DeleteByRestaurantIdAsync(restaurantId, cancellationToken);

        // Create new business hours
        var businessHours = businessHoursDtos.Select(dto => new BusinessHours
        {
            Id = Guid.NewGuid(),
            RestaurantId = restaurantId,
            DayOfWeek = dto.DayOfWeek,
            Status = dto.Status,
            OpenTime = dto.OpenTime,
            CloseTime = dto.CloseTime,
            BreakStartTime = dto.BreakStartTime,
            BreakEndTime = dto.BreakEndTime,
            Notes = dto.Notes
        });

        await _businessHoursRepository.AddRangeAsync(businessHours, cancellationToken);

        // Refresh restaurant data
        restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        return restaurant == null ? null : MapToRestaurantDto(restaurant);
    }

    public async Task<bool> IsRestaurantOpenAsync(Guid restaurantId, DateTime? dateTime = null, CancellationToken cancellationToken = default)
    {
        return await _businessHoursRepository.IsRestaurantOpenAtAsync(restaurantId, dateTime ?? DateTime.Now, cancellationToken);
    }

    public async Task<bool> RestaurantExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _restaurantRepository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> OwnerCanManageRestaurantAsync(Guid restaurantId, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        return restaurant?.OwnerId == ownerId;
    }

    #region Private Helper Methods

    private static RestaurantDto MapToRestaurantDto(Restaurant restaurant)
    {
        return new RestaurantDto(
            restaurant.Id,
            restaurant.Name,
            restaurant.Description,
            restaurant.CuisineType,
            restaurant.Status,
            restaurant.OwnerId,
            restaurant.PhoneNumber,
            restaurant.Email,
            restaurant.IsAcceptingOrders,
            restaurant.DeliveryFeePerKm,
            restaurant.MaxDeliveryDistanceKm,
            restaurant.MinimumOrderAmount,
            restaurant.CreatedAt,
            restaurant.UpdatedAt,
            MapToRestaurantLocationDto(restaurant.Location),
            restaurant.BusinessHours.Select(MapToBusinessHoursDto).ToList()
        );
    }

    private static RestaurantSummaryDto MapToRestaurantSummaryDto(Restaurant restaurant)
    {
        return new RestaurantSummaryDto(
            restaurant.Id,
            restaurant.Name,
            restaurant.Description,
            restaurant.CuisineType,
            restaurant.Status,
            restaurant.IsAcceptingOrders,
            restaurant.DeliveryFeePerKm,
            MapToRestaurantLocationDto(restaurant.Location)
        );
    }

    private static RestaurantLocationDto MapToRestaurantLocationDto(RestaurantLocation location)
    {
        return new RestaurantLocationDto(
            location.Id,
            location.Address,
            location.City,
            location.District,
            location.Ward,
            location.Latitude,
            location.Longitude,
            location.UpdatedAt
        );
    }

    private static BusinessHoursDto MapToBusinessHoursDto(BusinessHours businessHours)
    {
        return new BusinessHoursDto(
            businessHours.Id,
            businessHours.DayOfWeek,
            businessHours.Status,
            businessHours.OpenTime,
            businessHours.CloseTime,
            businessHours.BreakStartTime,
            businessHours.BreakEndTime,
            businessHours.Notes,
            businessHours.GetFormattedHours()
        );
    }

    #endregion
}