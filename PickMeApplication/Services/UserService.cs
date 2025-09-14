using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PickMeApplication.Models;
using PickMeApplication.Models.Dtos;
using PickMeApplication.Repositories;

namespace PickMeApplication.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;

    public UserService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var existingUserByUsername = await _userRepository.GetByUsernameAsync(createUserDto.Username);
        if (existingUserByUsername != null)
        {
            throw new InvalidOperationException($"Username '{createUserDto.Username}' is already taken.");
        }
        
        var existingUserByEmail = await _userRepository.GetByEmailAsync(createUserDto.Email);
        if (existingUserByEmail != null)
        {
            throw new InvalidOperationException($"Email '{createUserDto.Email}' is already registered.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(updateUserDto.Email))
        {
            // Check if new email is unique
            var existingUserWithEmail = await _userRepository.GetByEmailAsync(updateUserDto.Email);
            if (existingUserWithEmail != null && existingUserWithEmail.Id != id)
            {
                throw new InvalidOperationException($"Email '{updateUserDto.Email}' is already registered.");
            }
            user.Email = updateUserDto.Email;
        }

        if (!string.IsNullOrEmpty(updateUserDto.FirstName))
        {
            user.FirstName = updateUserDto.FirstName;
        }

        if (!string.IsNullOrEmpty(updateUserDto.LastName))
        {
            user.LastName = updateUserDto.LastName;
        }

        if (!string.IsNullOrEmpty(updateUserDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
        }

        if (updateUserDto.IsActive.HasValue)
        {
            user.IsActive = updateUserDto.IsActive.Value;
        }

        await _userRepository.UpdateAsync(user);
        return MapToDto(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }

    public async Task<UserLoginResponseDto?> AuthenticateAsync(UserLoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

        if (user == null || !user.IsActive)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return null;

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        
        var token = GenerateJwtToken(user);
        var expiryTime = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        return new UserLoginResponseDto(
            user.Id,
            user.Username,
            token,
            expiryTime
        );
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.CreatedAt,
            user.LastLoginAt,
            user.IsActive
        );
    }
}