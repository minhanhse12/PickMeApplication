using PickMeApplication.RestaurantService.Domain.Enums;

namespace PickMeApplication.RestaurantService.Domain.Entities;

public class BusinessHours
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    
    // Day of week (0 = Sunday, 1 = Monday, ..., 6 = Saturday)
    public int DayOfWeek { get; set; }
    
    public BusinessDayStatus Status { get; set; } = BusinessDayStatus.Open;
    
    // Opening hours
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    
    // Break time (optional)
    public TimeOnly? BreakStartTime { get; set; }
    public TimeOnly? BreakEndTime { get; set; }
    
    // Special notes
    public string? Notes { get; set; }
    
    // Navigation Property
    public Restaurant Restaurant { get; set; } = null!;
    
    // Domain Methods
    public bool IsOpenAt(TimeOnly time)
    {
        if (Status != BusinessDayStatus.Open)
            return false;
            
        // No break time
        if (!BreakStartTime.HasValue || !BreakEndTime.HasValue)
        {
            return time >= OpenTime && time <= CloseTime;
        }
        
        // Has break time - check if open before break or after break
        return (time >= OpenTime && time < BreakStartTime) ||
               (time >= BreakEndTime && time <= CloseTime);
    }
    
    public string GetFormattedHours()
    {
        if (Status == BusinessDayStatus.Closed)
            return "Closed";
            
        if (Status == BusinessDayStatus.Holiday)
            return "Holiday";
        
        var hours = $"{OpenTime:HH:mm} - {CloseTime:HH:mm}";
        
        if (BreakStartTime.HasValue && BreakEndTime.HasValue)
        {
            hours += $" (Break: {BreakStartTime:HH:mm} - {BreakEndTime:HH:mm})";
        }
        
        return hours;
    }
    
    public void SetBreakTime(TimeOnly breakStart, TimeOnly breakEnd)
    {
        if (breakStart >= breakEnd)
            throw new ArgumentException("Break start time must be before break end time");
            
        if (breakStart < OpenTime || breakEnd > CloseTime)
            throw new ArgumentException("Break time must be within opening hours");
            
        BreakStartTime = breakStart;
        BreakEndTime = breakEnd;
    }
    
    public void RemoveBreakTime()
    {
        BreakStartTime = null;
        BreakEndTime = null;
    }
    
    public static BusinessHours CreateDefault(int dayOfWeek, Guid restaurantId)
    {
        return new BusinessHours
        {
            Id = Guid.NewGuid(),
            RestaurantId = restaurantId,
            DayOfWeek = dayOfWeek,
            Status = BusinessDayStatus.Open,
            OpenTime = new TimeOnly(9, 0),  // 9:00 AM
            CloseTime = new TimeOnly(22, 0) // 10:00 PM
        };
    }
}