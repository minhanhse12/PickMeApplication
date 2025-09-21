namespace PickMeApplication.UserService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Domain methods
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
    
    public void Activate()
    {
        IsActive = true;
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
    
    public bool CanLogin()
    {
        return IsActive;
    }
}