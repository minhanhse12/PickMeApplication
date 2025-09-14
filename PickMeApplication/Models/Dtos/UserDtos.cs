namespace PickMeApplication.Models.Dtos;

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

public record CreateUserDto(
    string Username,
    string Email,
    string Password,
    string Role,
    string FirstName,
    string LastName
);

public record UpdateUserDto(
    string? Email = null,
    string? FirstName = null,
    string? LastName = null,
    string? Password = null,
    bool? IsActive = null
);

public record UserLoginDto(
    string Username,
    string Password
);

public record UserLoginResponseDto(
    Guid UserId,
    string Username,
    string Token,
    DateTime ExpiresAt
);