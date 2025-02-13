using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UsersApp.Infrastructure;
using UsersApp.Services;
using UsersApp.Api;
using UsersApp.Domain;
using UsersApp.Infrastructure.Authentication;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSerilog(lc => lc
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

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
;

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