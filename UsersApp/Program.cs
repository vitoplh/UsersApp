using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using UsersApp.Services;
using UsersApp.Api;
using UsersApp.Api.Authentication;
using UsersApp.Database;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSerilog(lc => lc
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)
        .UseAsyncSeeding(async (context, _, ct) =>
        {
            var apiClient = new ApiClient()
            {
                Id = 0,
                Key = "12345",
                Name = "Test client"
            };
            var contains = await context.Set<ApiClient>().CountAsync();

            if (contains == 0)
            {
                context.Set<ApiClient>().Add(apiClient);
                await context.SaveChangesAsync();
            }
        })
    );


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordHasher<string>, PasswordHasher<string>>();

var app = builder.Build();

await using (var serviceScope = app.Services.CreateAsyncScope())
await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>())
{
    await dbContext.Database.EnsureCreatedAsync();
    
}

app.UseHttpsRedirection();
app.MapUsers();
app.Run();

public partial class Program
{ }