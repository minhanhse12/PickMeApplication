namespace PickMeApplication.RestaurantService.Domain.Entities;

public class RestaurantLocation
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    
    // Address Information  
    public string Address { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    
    // GPS Coordinates
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    // Additional Location Info
    public string? Landmarks { get; set; }
    public string? DirectionsNote { get; set; }
    public string? ParkingInfo { get; set; }
    
    // Timestamps
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Property
    public Restaurant Restaurant { get; set; } = null!;
    
    // Domain Methods
    public string GetFormattedAddress()
    {
        return $"{Address}, {Ward}, {District}, {City}";
    }
    
    public void UpdateCoordinates(double lat, double lng)
    {
        Latitude = lat;
        Longitude = lng;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateAddress(string address, string ward, string district, string city, string postalCode = "")
    {
        Address = address;
        Ward = ward;
        District = district;
        City = city;
        PostalCode = postalCode;
        UpdatedAt = DateTime.UtcNow;
    }
    
    // Distance calculation using Haversine formula
    public double CalculateDistanceTo(double targetLat, double targetLng)
    {
        // Haversine formula for basic distance calculation
        const double R = 6371; // Earth's radius in km
        var dLat = ToRadians(targetLat - Latitude);
        var dLng = ToRadians(targetLng - Longitude);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(targetLat)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = R * c;
        
        return distance;
    }
    
    private static double ToRadians(double angle)
    {
        return angle * (Math.PI / 180);
    }
    
    public bool IsWithinDeliveryRange(double targetLat, double targetLng, double maxDistanceKm)
    {
        var distance = CalculateDistanceTo(targetLat, targetLng);
        return distance <= maxDistanceKm;
    }
}