using PickMeApplication.UserService.Application.DTOs;

namespace PickMeApplication.UserService.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LoginResponseDto?> AuthenticateAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}