using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SampleApp.Infrastructure;
using SampleApp.Services;
using SampleApp.Api;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSerilog(lc => lc
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordHasher<string>, PasswordHasher<string>>();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapUsers();
app.Run();