using PickMeApplication.RestaurantService.Domain.Enums;

namespace PickMeApplication.RestaurantService.Domain.Entities;

public class Restaurant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CuisineType CuisineType { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public RestaurantStatus Status { get; set; } = RestaurantStatus.Pending;
    
    // Owner Information (from User Service)
    public Guid OwnerId { get; set; }
    
    // Business Information
    public bool IsCurrentlyOpen { get; set; } = false;
    public bool IsAcceptingOrders { get; set; } = true;
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    public int TotalOrders { get; set; } = 0;
    
    // Delivery Settings
    public decimal DeliveryFeePerKm { get; set; } = 5000m;
    public double MaxDeliveryDistanceKm { get; set; } = 10.0;
    public decimal MinimumOrderAmount { get; set; } = 50000m;
    
    // Media
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public List<string> GalleryImageUrls { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public RestaurantLocation? Location { get; set; }
    public ICollection<BusinessHours> BusinessHours { get; set; } = new List<BusinessHours>();
    
    // Domain Methods
    public void UpdateStatus(RestaurantStatus newStatus, string? reason = null)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ToggleOrderAcceptance(string? reason = null)
    {
        IsAcceptingOrders = !IsAcceptingOrders;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateRating(decimal newRating, int reviewCount)
    {
        AverageRating = newRating;
        TotalReviews = reviewCount;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool IsOpenAt(DateTime dateTime)
    {
        var dayOfWeek = (int)dateTime.DayOfWeek;
        var timeOnly = TimeOnly.FromDateTime(dateTime);
        
        var todayHours = BusinessHours
            .FirstOrDefault(bh => bh.DayOfWeek == dayOfWeek && bh.Status == BusinessDayStatus.Open);
        
        if (todayHours == null) return false;
        
        return todayHours.IsOpenAt(timeOnly);
    }
    
    public decimal CalculateDeliveryFee(double distanceKm)
    {
        if (distanceKm <= 0) return 0;
        if (distanceKm > MaxDeliveryDistanceKm) return -1; // Cannot deliver
        
        return DeliveryFeePerKm * (decimal)distanceKm;
    }
    
    public void IncrementOrderCount()
    {
        TotalOrders++;
        UpdatedAt = DateTime.UtcNow;
    }
}