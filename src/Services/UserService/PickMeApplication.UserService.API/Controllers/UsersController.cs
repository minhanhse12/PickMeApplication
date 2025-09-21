using Microsoft.AspNetCore.Mvc;
using PickMeApplication.UserService.Application.DTOs;
using PickMeApplication.UserService.Application.Interfaces;

namespace PickMeApplication.UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.CreateUserAsync(createUserDto, cancellationToken);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, updateUserDto, cancellationToken);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id, cancellationToken);
            if (!result)
                return NotFound($"User with ID {id} not found.");

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Authenticate user and get JWT token
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userService.AuthenticateAsync(loginDto, cancellationToken);
            if (result == null)
                return Unauthorized("Invalid email or password.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("{id:guid}/change-password")]
    public async Task<ActionResult> ChangePassword(
        Guid id, 
        [FromBody] ChangePasswordDto changePasswordDto, 
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userService.ChangePasswordAsync(
                id, 
                changePasswordDto.CurrentPassword, 
                changePasswordDto.NewPassword, 
                cancellationToken);
            
            if (!result)
                return BadRequest("Current password is incorrect or user not found.");

            return Ok("Password changed successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public record ChangePasswordDto(string CurrentPassword, string NewPassword);