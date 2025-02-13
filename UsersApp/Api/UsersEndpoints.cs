using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsersApp.Mapping;
using UsersApp.Api.Requests;
using UsersApp.Api.Responses;
using UsersApp.Infrastructure.Authentication;
using UsersApp.Services;

namespace UsersApp.Api;

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
    static async Task<Results<Ok<UserDataResponse>, NotFound>> GetUser(HttpContext context, string username, 
        IUserService userService, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("UsersEndpoints");
        logger.LogInformation("Host {hostname} - Client {clientName} (IP {ip}) - GetUser: {username}", Environment.MachineName, 
            context.Items["ClientName"], context.Connection.RemoteIpAddress, username);
        
        var user = await userService.GetUser(username);
        if (user is not null)
        {
            return TypedResults.Ok(user.ToUserResponse());
        }
        return TypedResults.NotFound();
    }

    static async Task<Results<Created<UserDataResponse>, Conflict<ProblemDetails>, InternalServerError>> 
        CreateUser(HttpContext context, CreateUserRequest request, IUserService userService, 
            IPasswordHasher<string> passwordHasher, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("UsersEndpoints");
        logger.LogInformation("Host {hostname} - Client {clientName} (IP {ip}) - CreateUser: {user}", Environment.MachineName, 
            context.Items["ClientName"], context.Connection.RemoteIpAddress, request.ToString());
        
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
    
    static async Task<Results<Ok<UserDataResponse>, NotFound, InternalServerError>> UpdateUser(HttpContext context, 
        string username, UpdateUserRequest request, IUserService userService, IPasswordHasher<string> passwordHasher,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("UsersEndpoints");
        logger.LogInformation("Host {hostname} - Client {clientName} (IP {ip}) - UpdateUser: {username} Data: {userdata}", 
            Environment.MachineName, context.Items["ClientName"], context.Connection.RemoteIpAddress, username, request.ToString());
        
        var updatedUser = request.ToUser(username, passwordHasher);
        var result = await userService.UpdateUser(updatedUser);
        return result switch
        {
            UserServiceResult.Success => TypedResults.Ok(updatedUser.ToUserResponse()),
            UserServiceResult.NotFound => TypedResults.NotFound(),
            _ => TypedResults.InternalServerError()
        };
    }
    
    static async Task<Results<Ok, NotFound, InternalServerError>> DeleteUser(HttpContext context, string username, 
        IUserService userService, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("UsersEndpoints");
        logger.LogInformation("Host {hostname} - Client {clientName} (IP {ip}) - DeleteUser: {username}", Environment.MachineName, 
            context.Items["ClientName"], context.Connection.RemoteIpAddress, username);
        
        var result = await userService.DeleteUser(username);
        return result switch
        {
            UserServiceResult.Success => TypedResults.Ok(),
            UserServiceResult.NotFound => TypedResults.NotFound(),
            _ => TypedResults.InternalServerError()
        };
    }

    static async Task<Results<Ok<PasswordValidationResponse>, NotFound, InternalServerError>> 
        ValidatePassword(HttpContext context, string username, string password, IUserService userService, 
            IPasswordHasher<string> passwordHasher, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("UsersEndpoints");
        logger.LogInformation("Host {hostname} - Client {clientName} (IP {ip}) - VerifyPassword: {username} - " +
                              "Password hash: {hash}", Environment.MachineName, 
            context.Items["ClientName"], context.Connection.RemoteIpAddress, username, 
            passwordHasher.HashPassword(username, password));
        
        return await userService.ValidatePassword(username, password, passwordHasher) switch
        {
            UserServiceResult.Success => TypedResults.Ok(new PasswordValidationResponse(true)),
            UserServiceResult.Failed => TypedResults.Ok(new PasswordValidationResponse(false)),
            UserServiceResult.NotFound => TypedResults.NotFound(),
            _ => TypedResults.InternalServerError()
        };
    }
}