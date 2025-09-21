namespace PickMeApplication.UserService.Application.DTOs;

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
    string FirstName,
    string LastName,
    string Password,
    string Role = "Customer"
);

public record UpdateUserDto(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Role
);

public record LoginDto(
    string Email,
    string Password
);

public record LoginResponseDto(
    string Token,
    UserDto User
);