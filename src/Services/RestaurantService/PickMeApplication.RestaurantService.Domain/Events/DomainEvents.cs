using PickMeApplication.RestaurantService.Domain.Entities;
using PickMeApplication.RestaurantService.Domain.Enums;

namespace PickMeApplication.RestaurantService.Domain.Events;

public abstract class DomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Restaurant Events
public class RestaurantCreatedEvent : DomainEvent
{
    public Guid RestaurantId { get; }
    public Guid OwnerId { get; }
    public string Name { get; }

    public RestaurantCreatedEvent(Guid restaurantId, Guid ownerId, string name)
    {
        RestaurantId = restaurantId;
        OwnerId = ownerId;
        Name = name;
    }
}

public class RestaurantStatusChangedEvent : DomainEvent
{
    public Guid RestaurantId { get; }
    public RestaurantStatus FromStatus { get; }
    public RestaurantStatus ToStatus { get; }
    public string? Reason { get; }

    public RestaurantStatusChangedEvent(Guid restaurantId, RestaurantStatus fromStatus, RestaurantStatus toStatus, string? reason = null)
    {
        RestaurantId = restaurantId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        Reason = reason;
    }
}

public class RestaurantOrderAcceptanceChangedEvent : DomainEvent
{
    public Guid RestaurantId { get; }
    public bool IsAcceptingOrders { get; }
    public string? Reason { get; }

    public RestaurantOrderAcceptanceChangedEvent(Guid restaurantId, bool isAcceptingOrders, string? reason = null)
    {
        RestaurantId = restaurantId;
        IsAcceptingOrders = isAcceptingOrders;
        Reason = reason;
    }
}

public class RestaurantLocationUpdatedEvent : DomainEvent
{
    public Guid RestaurantId { get; }
    public double? OldLatitude { get; }
    public double? OldLongitude { get; }
    public double NewLatitude { get; }
    public double NewLongitude { get; }
    public string NewAddress { get; }

    public RestaurantLocationUpdatedEvent(
        Guid restaurantId, 
        double? oldLatitude, 
        double? oldLongitude, 
        double newLatitude, 
        double newLongitude, 
        string newAddress)
    {
        RestaurantId = restaurantId;
        OldLatitude = oldLatitude;
        OldLongitude = oldLongitude;
        NewLatitude = newLatitude;
        NewLongitude = newLongitude;
        NewAddress = newAddress;
    }
}

// Business Hours Events
public class BusinessHoursUpdatedEvent : DomainEvent
{
    public Guid RestaurantId { get; }
    public IReadOnlyList<BusinessHours> UpdatedBusinessHours { get; }

    public BusinessHoursUpdatedEvent(Guid restaurantId, IEnumerable<BusinessHours> updatedBusinessHours)
    {
        RestaurantId = restaurantId;
        UpdatedBusinessHours = updatedBusinessHours.ToList().AsReadOnly();
    }
}

public class RestaurantOpeningStatusChangedEvent : DomainEvent
{
    public Guid RestaurantId { get; }
    public bool IsCurrentlyOpen { get; }
    public DateTime CheckedAt { get; }

    public RestaurantOpeningStatusChangedEvent(Guid restaurantId, bool isCurrentlyOpen, DateTime checkedAt)
    {
        RestaurantId = restaurantId;
        IsCurrentlyOpen = isCurrentlyOpen;
        CheckedAt = checkedAt;
    }
}