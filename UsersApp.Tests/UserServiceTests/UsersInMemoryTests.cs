using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using UsersApp.Domain;
using UsersApp.Infrastructure;
using UsersApp.Services;

namespace UsersApp.Tests.UserServiceTests;

public class UsersInMemoryTests
{
    private readonly DbContextOptions<AppDbContext> _contextOptions;

    public UsersInMemoryTests()
    {
        DbConnection connection =
            new SqliteConnection("Filename=:memory:");
        connection.Open();

        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(_contextOptions);

        context.Database.EnsureCreated();
        
        context.AddRange(
            new User
            {
                Id = 0,
                Username = "username123",
                Fullname = "Janez Novak",
                Email = "janeznovak@test.com",
                MobileNumber = "0038641582862",
                Language = "si",
                Culture = "si_SI",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PasswordHash = "passwordHash123==",
            });
        context.SaveChanges();
    }
    
    AppDbContext CreateContext() => new AppDbContext(_contextOptions);
        
    
    [Fact]
    public async Task GetUserReturnsNotFoundIfNotExists()
    {
        await using var context = CreateContext();
        
        var userService = new UserService(context);
        
        var result = await userService.GetUser("username999");
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetUserReturnsUser()
    {
        await using var context = CreateContext();
        
        var userService = new UserService(context);
        
        var result = await userService.GetUser("username123");
        
        Assert.NotNull(result);
        Assert.IsType<User>(result);
    }
    
    [Fact]
    public async Task CreateUserReturnsAlreadyExists()
    {
        await using var context = CreateContext();

        var user = new User
        {
            Username = "username123",
            Fullname = "Janez Novak",
            Email = "janeznovak@test.com",
            MobileNumber = "0038641582862",
            Language = "si",
            Culture = "si_SI",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            PasswordHash = "passwordHash123=="
        };
        
        var userService = new UserService(context);
        
        var result = await userService.CreateUser(user);

        Assert.IsType<UserServiceResult>(result);
        Assert.Equal(UserServiceResult.AlreadyExists, result);
    }
    
    [Fact]
    public async Task CreateUserReturnsEmailInUse()
    {
        await using var context = CreateContext();

        var user = new User
        {
            Username = "username456",
            Fullname = "Janez Novak",
            Email = "janeznovak@test.com",
            MobileNumber = "0038641582862",
            Language = "si",
            Culture = "si_SI",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            PasswordHash = "passwordHash123=="
        };
        
        var userService = new UserService(context);
        
        var result = await userService.CreateUser(user);

        Assert.IsType<UserServiceResult>(result);
        Assert.Equal(UserServiceResult.EmailInUse, result);
    }
    
    [Fact]
    public async Task CreateUserCreatesUserInDatabase()
    {
        await using var context = CreateContext();
        
        var user = new User
        {
            Username = "username679",
            Fullname = "Janez Horvat",
            Email = "janezhorvat@test.com",
            MobileNumber = "0038641582862",
            Language = "si",
            Culture = "si_SI",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            PasswordHash = "passwordHash123=="
        };
        
        var userService = new UserService(context);
        
        var result = await userService.CreateUser(user);

        Assert.IsType<UserServiceResult>(result);
        Assert.Equal(UserServiceResult.Success, result);
        
        var dbUser = await userService.GetUser(user.Username);
        
        Assert.NotNull(dbUser);
        Assert.NotNull(dbUser.Id);
        Assert.Equal(user.Username, dbUser.Username);
        Assert.Equal(user.Fullname, dbUser.Fullname);
        Assert.Equal(user.Email, dbUser.Email);
        Assert.Equal(user.MobileNumber, dbUser.MobileNumber);
        Assert.Equal(user.Language, dbUser.Language);
        Assert.Equal(user.Culture, dbUser.Culture);
        Assert.Equal(user.CreatedAt, dbUser.CreatedAt);
        Assert.Equal(user.UpdatedAt, dbUser.UpdatedAt);
        Assert.Equal(user.PasswordHash, dbUser.PasswordHash);
        Assert.False(dbUser.IsDeleted);
    }
    
    [Fact]
    public async Task UpdateUserReturnsNotFoundIfNotExists()
    {
        await using var context = CreateContext();
        
        var user = new User
        {
            Username = "username777",
            Fullname = "Lojze Novak",
            Email = "lojzenovak@test.com",
            MobileNumber = "0038641582862",
            Language = "si",
            Culture = "si_SI",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            PasswordHash = "passwordHash123=="
        };
        
        var userService = new UserService(context);
        
        var result = await userService.UpdateUser(user);

        Assert.IsType<UserServiceResult>(result);
        Assert.Equal(UserServiceResult.NotFound, result);
    }
    
    [Fact]
    public async Task UpdateUserReturnsUpdatedUser()
    {
        await using var context = CreateContext();
        
        var user = new User
        {
            Username = "username123",
            Fullname = "Lojze Novak",
            Email = "lojzenovak@test.com",
            MobileNumber = "0038641582862",
            Language = "si",
            Culture = "si_SI",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            PasswordHash = "passwordHash123=="
        };
        
        var userService = new UserService(context);
        
        var result = await userService.UpdateUser(user);

        Assert.IsType<UserServiceResult>(result);
        Assert.Equal(UserServiceResult.Success, result);
        
        var dbUser = await userService.GetUser(user.Username);
        
        Assert.Equal(user.Fullname, dbUser.Fullname);
        Assert.Equal(user.Email, dbUser.Email);
    }
    
    [Fact]
    public async Task DeleteUserReturnsSuccess()
    {
        await using var context = CreateContext();

        var username = "username123";
        
        var userService = new UserService(context);
        
        var result = await userService.DeleteUser(username);
        
        Assert.IsType<UserServiceResult>(result);
        Assert.Equal(UserServiceResult.Success, result);

        var dbUser = await context.Users.AsNoTracking().SingleOrDefaultAsync(u =>
            (u.Username == username && u.IsDeleted == true));
        
        Assert.NotNull(dbUser);
    }
    
    [Fact]
    public async Task DeleteUserReturnsNotFoundIfNotExists()
    {
        await using var context = CreateContext();
        
        var userService = new UserService(context);
        
        var result = await userService.DeleteUser("usernameNot");
        
        Assert.IsType<UserServiceResult>(result);
        Assert.Equal(UserServiceResult.NotFound, result);
    }
    
    [Fact]
    public async Task VerifyPasswordReturnsUserNotFoundIfNotExists()
    {
        var passwordHasher = Substitute.For<IPasswordHasher<string>>();
        passwordHasher.VerifyHashedPassword("username123", "passwordHash123==","test123")
            .Returns(PasswordVerificationResult.Success);
        
        await using var context = CreateContext();
        
        var userService = new UserService(context);
        
        var result = await userService.ValidatePassword("usernameXXX","test123", passwordHasher);

        Assert.IsType<UserServiceResult>(result);
        Assert.Equal(UserServiceResult.NotFound, result);
    }
    
    [Fact]
    public async Task VerifyPasswordReturnsSuccessIfMatches()
    {
        var passwordHasher = Substitute.For<IPasswordHasher<string>>();
        passwordHasher.VerifyHashedPassword("username123", "passwordHash123==","test123")
            .Returns(PasswordVerificationResult.Success);
        
        await using var context = CreateContext();
        
        var userService = new UserService(context);
        
        var result = await userService.ValidatePassword("username123","test123", passwordHasher);

        Assert.IsType<UserServiceResult>(result);
        Assert.Equal(UserServiceResult.Success, result);
    }
    
    [Fact]
    public async Task VerifyPasswordReturnsFailedIfNotMatches()
    {
        var passwordHasher = Substitute.For<IPasswordHasher<string>>();
        passwordHasher.VerifyHashedPassword("username123", "passwordHash123==","test456")
            .Returns(PasswordVerificationResult.Failed);
        
        await using var context = CreateContext();
        
        var userService = new UserService(context);
        
        var result = await userService.ValidatePassword("username123","test456", passwordHasher);

        Assert.IsType<UserServiceResult>(result);
        Assert.Equal(UserServiceResult.Failed, result);
    }
}