using Microsoft.AspNetCore.Mvc;
using PickMeApplication.Models.Dtos;
using PickMeApplication.Services;

namespace PickMeApplication.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/users")
            .WithTags("Users")
            .WithOpenApi();
        
        userGroup.MapGet("/", async (IUserService userService) =>
        {
            try
            {
                var users = await userService.GetAllUsersAsync();
                return Results.Ok(users);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }).RequireAuthorization();

        // GET /api/users/{id} - Get a user by ID
        userGroup.MapGet("/{id:guid}", async (IUserService userService, Guid id) =>
        {
            try
            {
                var user = await userService.GetUserByIdAsync(id);
                if (user == null)
                    return Results.NotFound($"User with ID {id} not found.");

                return Results.Ok(user);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }).RequireAuthorization();
        
        userGroup.MapPost("/", async (IUserService userService, [FromBody] CreateUserDto createUserDto) =>
        {
            try
            {
                var user = await userService.CreateUserAsync(createUserDto);
                return Results.Created($"/api/users/{user.Id}", user);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
        
        userGroup.MapPut("/{id:guid}", async (IUserService userService, Guid id, [FromBody] UpdateUserDto updateUserDto) =>
        {
            try
            {
                var user = await userService.UpdateUserAsync(id, updateUserDto);
                if (user == null)
                    return Results.NotFound($"User with ID {id} not found.");

                return Results.Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }).RequireAuthorization();
        
        userGroup.MapDelete("/{id:guid}", async (IUserService userService, Guid id) =>
        {
            try
            {
                var result = await userService.DeleteUserAsync(id);
                if (!result)
                    return Results.NotFound($"User with ID {id} not found.");

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }).RequireAuthorization();
        
        userGroup.MapPost("/login", async (IUserService userService, [FromBody] UserLoginDto loginDto) =>
        {
            try
            {
                var response = await userService.AuthenticateAsync(loginDto);
                if (response == null)
                    return Results.Unauthorized();

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
        
        userGroup.MapGet("/profile", (HttpContext context) =>
        {
            var userId = context.User.FindFirst("sub")?.Value;
            if (userId == null)
                return Results.Unauthorized();

            return Results.Ok(new { UserId = userId });
        }).RequireAuthorization();
    }
}