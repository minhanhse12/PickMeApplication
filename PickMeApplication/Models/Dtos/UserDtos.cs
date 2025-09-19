using System.ComponentModel.DataAnnotations;

namespace PickMeApplication.Models.Dtos;

/// <summary>
/// Represents a user in the system
/// </summary>
public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    bool IsActive
);

/// <summary>
/// Data required to create a new user
/// </summary>
public record CreateUserDto(
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
    string Username,
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    string Password,
    
    [Required(ErrorMessage = "Role is required")]
    string Role,
    
    [Required(ErrorMessage = "First name is required")]
    string FirstName,
    
    [Required(ErrorMessage = "Last name is required")]
    string LastName
);

/// <summary>
/// Data for updating an existing user
/// </summary>
public record UpdateUserDto(
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string? Email = null,
    
    string? FirstName = null,
    
    string? LastName = null,
    
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    string? Password = null,
    
    bool? IsActive = null
);

/// <summary>
/// User login credentials
/// </summary>
public record UserLoginDto(
    [Required(ErrorMessage = "Username is required")]
    string Username,
    
    [Required(ErrorMessage = "Password is required")]
    string Password
);

/// <summary>
/// Response after successful login
/// </summary>
public record UserLoginResponseDto(
    Guid UserId,
    string Username,
    string Token,
    DateTime ExpiresAt
);