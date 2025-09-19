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
        
        // GET /api/users - Get all users
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
        })
        .WithName("GetAllUsers")
        .WithDescription("Retrieves all users")
        .RequireAuthorization();

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
        })
        .WithName("GetUserById")
        .WithDescription("Retrieves a specific user by their ID")
        .RequireAuthorization();
        
        // POST /api/users - Create a new user
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
        })
        .WithName("CreateUser")
        .WithDescription("Creates a new user account");
        
        // PUT /api/users/{id} - Update an existing user
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
        })
        .WithName("UpdateUser")
        .WithDescription("Updates an existing user's information")
        .RequireAuthorization();
        
        // DELETE /api/users/{id} - Delete a user
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
        })
        .WithName("DeleteUser")
        .WithDescription("Deletes a user account")
        .RequireAuthorization();
        
        // POST /api/users/login - Authenticate a user
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
        })
        .WithName("LoginUser")
        .WithDescription("Authenticates a user and returns a JWT token");
        
        // GET /api/users/profile - Get current user profile
        userGroup.MapGet("/profile", (HttpContext context) =>
        {
            var userId = context.User.FindFirst("sub")?.Value;
            if (userId == null)
                return Results.Unauthorized();

            return Results.Ok(new { UserId = userId });
        })
        .WithName("GetUserProfile")
        .WithDescription("Retrieves the profile of the currently authenticated user")
        .RequireAuthorization();
    }
}