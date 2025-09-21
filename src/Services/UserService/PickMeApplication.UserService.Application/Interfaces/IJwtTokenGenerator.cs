using PickMeApplication.UserService.Application.DTOs;

namespace PickMeApplication.UserService.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserDto user);
}