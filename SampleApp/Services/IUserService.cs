using Microsoft.AspNetCore.Identity;
using SampleApp.Domain;

namespace SampleApp.Services;

public interface IUserService
{
    Task<User?> GetUser(string username);
    
    Task<UserServiceResult> CreateUser(User user);

    Task<UserServiceResult> UpdateUser(User user);
    
    Task<UserServiceResult> DeleteUser(string username);
    
    Task<UserServiceResult> ValidatePassword(string username, string password, IPasswordHasher<string> passwordHasher);
}