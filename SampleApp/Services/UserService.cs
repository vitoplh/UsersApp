using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleApp.Domain;
using SampleApp.Infrastructure;

namespace SampleApp.Services;

public class UserService(AppDbContext dbContext) : IUserService
{
    public async Task<User?> GetUser(string username)
    {
        return await dbContext.Users.AsNoTracking().SingleOrDefaultAsync(u =>
            (u.Username == username && u.IsDeleted == false));
    }

    public async Task<UserServiceResult> CreateUser(User user)
    {
        var existingUser = await dbContext.Users.SingleOrDefaultAsync(u => 
            (u.Username == user.Username || u.Email == user.Email) && u.IsDeleted == false);
        
        if (existingUser is not null)
        {
            return existingUser.Username == user.Username ? UserServiceResult.AlreadyExists : UserServiceResult.EmailInUse;
        }

        await dbContext.Users.AddAsync(user);
        return await dbContext.SaveChangesAsync() > 0 ? UserServiceResult.Success : UserServiceResult.Failed;
    }

    public async Task<UserServiceResult> UpdateUser(User user)
    {
        var rowsAffected = await dbContext.Users.Where(u => u.Username == user.Username && u.IsDeleted == false)
            .ExecuteUpdateAsync(setters => 
                setters.SetProperty(u => u.Username, user.Username)
                    .SetProperty(u => u.Fullname, user.Fullname)
                    .SetProperty(u => u.Email, user.Email)
                    .SetProperty(u => u.MobileNumber, user.MobileNumber)
                    .SetProperty(u => u.Language, user.Language)
                    .SetProperty(u => u.Culture, user.Culture)
                    .SetProperty(u => u.PasswordHash, user.PasswordHash)
                    .SetProperty(u => u.UpdatedAt, DateTime.Now));
        
        return rowsAffected == 0 ? UserServiceResult.NotFound : UserServiceResult.Success;
    }

    public async Task<UserServiceResult> DeleteUser(string username)
    {
        var rowsAffected = await dbContext.Users.Where(u => u.Username == username && u.IsDeleted == false)
            .ExecuteUpdateAsync(setters =>
            setters.SetProperty(u => u.IsDeleted, true)
                    .SetProperty(u => u.UpdatedAt, DateTime.UtcNow));
        
        return rowsAffected == 0 ? UserServiceResult.NotFound : UserServiceResult.Success;
    }

    public async Task<UserServiceResult> ValidatePassword(string username, string password, IPasswordHasher<string> passwordHasher)
    {
        var storedPasswordHash = await dbContext.Users.Where(u => u.Username == username && u.IsDeleted == false)
            .Select(u => u.PasswordHash).SingleOrDefaultAsync();
        
        if (storedPasswordHash is null)
        {
            return UserServiceResult.NotFound;
        }
        
        var verificationResult = passwordHasher.VerifyHashedPassword(username, storedPasswordHash, password);
        
        return verificationResult == PasswordVerificationResult.Success ? UserServiceResult.Success : UserServiceResult.Failed;
    }
}