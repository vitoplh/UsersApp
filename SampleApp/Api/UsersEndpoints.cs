using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SampleApp.Api.Requests;
using SampleApp.Api.Responses;
using SampleApp.Infrastructure.Authentication;
using SampleApp.Mapping;
using SampleApp.Services;

namespace SampleApp.Api;

internal static class UsersEndpoints
{
    public static IEndpointRouteBuilder  MapUsers(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .AddEndpointFilter<ApiKeyFilter>();

        group.MapGet("/{username}", GetUser);
        group.MapPost("/", CreateUser);
        group.MapPut("{username}", UpdateUser);
        group.MapDelete("{username}", DeleteUser);
        group.MapPost("{username}", ValidatePassword);
        
        return group;
    }
    static async Task<Results<Ok<UserDataResponse>, NotFound>> GetUser(string username, IUserService userService)
    {
        var user = await userService.GetUser(username);
        if (user is not null)
        {
            return TypedResults.Ok(user.ToUserResponse());
        }
        return TypedResults.NotFound();
    }

    static async Task<Results<Created<UserDataResponse>, Conflict<ProblemDetails>, InternalServerError>> CreateUser(CreateUserRequest request, IUserService userService, IPasswordHasher<string> passwordHasher)
    {
        var newUser = request.ToUser(passwordHasher);
        var result = await userService.CreateUser(newUser);
        return result switch
        {
            UserServiceResult.Success => TypedResults.Created($"/users/{newUser.Username}",
                newUser.ToUserResponse()),
            UserServiceResult.AlreadyExists => TypedResults.Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict, Title = "User already exists",
            }),
            UserServiceResult.EmailInUse => TypedResults.Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict, Title = "E-mail is already in use",
            }),
            _ => TypedResults.InternalServerError()
        };
    }
    
    static async Task<Results<Ok<UserDataResponse>, NotFound, InternalServerError>> UpdateUser(string username, UpdateUserRequest request, IUserService userService, IPasswordHasher<string> passwordHasher)
    {
        var updatedUser = request.ToUser(username, passwordHasher);
        var result = await userService.UpdateUser(updatedUser);
        return result switch
        {
            UserServiceResult.Success => TypedResults.Ok(updatedUser.ToUserResponse()),
            UserServiceResult.NotFound => TypedResults.NotFound(),
            _ => TypedResults.InternalServerError()
        };
    }
    
    static async Task<Results<Ok, NotFound, InternalServerError>> DeleteUser(string username, IUserService userService)
    {
        var result = await userService.DeleteUser(username);
        return result switch
        {
            UserServiceResult.Success => TypedResults.Ok(),
            UserServiceResult.NotFound => TypedResults.NotFound(),
            _ => TypedResults.InternalServerError()
        };
    }

    static async Task<Results<Ok<PasswordValidationResponse>, NotFound, InternalServerError>> ValidatePassword(string username, string password, IUserService userService, IPasswordHasher<string> passwordHasher)
    {
        return await userService.ValidatePassword(username, password, passwordHasher) switch
        {
            UserServiceResult.Success => TypedResults.Ok(new PasswordValidationResponse(true)),
            UserServiceResult.Failed => TypedResults.Ok(new PasswordValidationResponse(false)),
            UserServiceResult.NotFound => TypedResults.NotFound(),
            _ => TypedResults.InternalServerError()
        };
    }
}