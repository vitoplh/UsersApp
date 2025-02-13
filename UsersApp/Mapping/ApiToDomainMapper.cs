 using Microsoft.AspNetCore.Identity;
 using UsersApp.Api.Requests;
using UsersApp.Domain;

namespace UsersApp.Mapping;

public static class ApiToDomainMapper
{
    public static User ToUser(this CreateUserRequest request, IPasswordHasher<string> passwordHasher)
    {
        var passwordHash = passwordHasher.HashPassword(request.Username, request.Password);
        
        return new User
        {
            Username = request.Username,
            Fullname = request.Fullname,
            Email = request.Email,
            MobileNumber = request.MobileNumber,
            Language = request.Language,
            Culture = request.Culture,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    
    public static User ToUser(this UpdateUserRequest request, string username, IPasswordHasher<string> passwordHasher)
    {
        var passwordHash = passwordHasher.HashPassword(username, request.Password);
        
        return new User
        {
            Username = username,
            Fullname = request.Fullname,
            Email = request.Email,
            MobileNumber = request.MobileNumber,
            Language = request.Language,
            Culture = request.Culture,
            PasswordHash = passwordHash,
        };
    }
}
