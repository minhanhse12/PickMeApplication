using PickMeApplication.RestaurantService.Domain.Enums;

namespace PickMeApplication.RestaurantService.Application.DTOs;

public record CreateRestaurantDto(
    string Name,
    string Description,
    CuisineType CuisineType,
    Guid OwnerId,
    string PhoneNumber,
    string Email,
    // Location
    string Address,
    string City,
    string District,
    string Ward,
    double Latitude,
    double Longitude,
    // Business settings
    decimal DeliveryFeePerKm = 5000m,
    double MaxDeliveryDistanceKm = 10.0,
    decimal MinimumOrderAmount = 50000m
);

public record UpdateRestaurantDto(
    string Name,
    string Description,
    CuisineType CuisineType,
    string PhoneNumber,
    string Email,
    decimal DeliveryFeePerKm,
    double MaxDeliveryDistanceKm,
    decimal MinimumOrderAmount
);

public record UpdateRestaurantLocationDto(
    string Address,
    string City,
    string District,
    string Ward,
    double Latitude,
    double Longitude
);

public record RestaurantDto(
    Guid Id,
    string Name,
    string Description,
    CuisineType CuisineType,
    RestaurantStatus Status,
    Guid OwnerId,
    string PhoneNumber,
    string Email,
    bool IsAcceptingOrders,
    decimal DeliveryFeePerKm,
    double MaxDeliveryDistanceKm,
    decimal MinimumOrderAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    RestaurantLocationDto Location,
    List<BusinessHoursDto> BusinessHours
);

public record RestaurantLocationDto(
    Guid Id,
    string Address,
    string City,
    string District,
    string Ward,
    double Latitude,
    double Longitude,
    DateTime UpdatedAt
);

public record CreateBusinessHoursDto(
    int DayOfWeek,
    BusinessDayStatus Status,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    TimeOnly? BreakStartTime = null,
    TimeOnly? BreakEndTime = null,
    string? Notes = null
);

public record UpdateBusinessHoursDto(
    Guid Id,
    BusinessDayStatus Status,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    TimeOnly? BreakStartTime = null,
    TimeOnly? BreakEndTime = null,
    string? Notes = null
);

public record BusinessHoursDto(
    Guid Id,
    int DayOfWeek,
    BusinessDayStatus Status,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    TimeOnly? BreakStartTime,
    TimeOnly? BreakEndTime,
    string? Notes,
    string FormattedHours
);

// Search and filter DTOs
public record RestaurantSearchDto(
    string? SearchTerm = null,
    CuisineType? CuisineType = null,
    bool? IsAcceptingOrders = null,
    double? Latitude = null,
    double? Longitude = null,
    double? RadiusInKm = null,
    RestaurantStatus? Status = null
);

public record RestaurantSummaryDto(
    Guid Id,
    string Name,
    string Description,
    CuisineType CuisineType,
    RestaurantStatus Status,
    bool IsAcceptingOrders,
    decimal DeliveryFeePerKm,
    RestaurantLocationDto Location,
    double? DistanceKm = null,
    decimal? EstimatedDeliveryFee = null,
    bool IsCurrentlyOpen = false
);

// Response DTOs
public record RestaurantCreatedResponse(
    Guid Id,
    string Name,
    string Message
);

public record RestaurantStatusResponse(
    Guid Id,
    RestaurantStatus Status,
    string Message
);

public record RestaurantOrderAcceptanceResponse(
    Guid Id,
    bool IsAcceptingOrders,
    string Message
);