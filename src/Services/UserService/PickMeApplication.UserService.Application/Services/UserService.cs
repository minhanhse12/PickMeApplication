using PickMeApplication.UserService.Application.DTOs;
using PickMeApplication.UserService.Application.Interfaces;
using PickMeApplication.UserService.Domain.Entities;
using PickMeApplication.UserService.Domain.Repositories;

namespace PickMeApplication.UserService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        if (await _userRepository.ExistsByEmailAsync(createUserDto.Email, cancellationToken))
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        if (await _userRepository.ExistsByUsernameAsync(createUserDto.Username, cancellationToken))
        {
            throw new InvalidOperationException("User with this username already exists.");
        }

        // Parse role
        if (!Enum.TryParse<UserRole>(createUserDto.Role, true, out var role))
        {
            throw new ArgumentException("Invalid role specified.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            PasswordHash = _passwordHasher.HashPassword(createUserDto.Password),
            Role = role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdUser = await _userRepository.AddAsync(user, cancellationToken);
        return MapToDto(createdUser);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        // Parse role
        if (!Enum.TryParse<UserRole>(updateUserDto.Role, true, out var role))
        {
            throw new ArgumentException("Invalid role specified.");
        }

        // Check for duplicate email and username (excluding current user)
        var existingUserByEmail = await _userRepository.GetByEmailAsync(updateUserDto.Email, cancellationToken);
        if (existingUserByEmail != null && existingUserByEmail.Id != id)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var existingUserByUsername = await _userRepository.GetByUsernameAsync(updateUserDto.Username, cancellationToken);
        if (existingUserByUsername != null && existingUserByUsername.Id != id)
        {
            throw new InvalidOperationException("User with this username already exists.");
        }

        // Update user properties
        user.Username = updateUserDto.Username;
        user.Email = updateUserDto.Email;
        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.Role = role;

        await _userRepository.UpdateAsync(user, cancellationToken);
        return MapToDto(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await _userRepository.ExistsAsync(id, cancellationToken))
        {
            return false;
        }

        await _userRepository.DeleteAsync(id, cancellationToken);
        return true;
    }

    public async Task<LoginResponseDto?> AuthenticateAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email, cancellationToken);
        
        if (user == null || !user.CanLogin() || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return null;
        }

        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);

        var userDto = MapToDto(user);
        var token = _jwtTokenGenerator.GenerateToken(userDto);

        return new LoginResponseDto(token, userDto);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        
        if (user == null || !_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        
        return true;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role.ToString(),
            user.CreatedAt,
            user.LastLoginAt,
            user.IsActive
        );
    }
}