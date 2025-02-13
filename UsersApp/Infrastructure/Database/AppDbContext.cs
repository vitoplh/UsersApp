using Microsoft.EntityFrameworkCore;
using UsersApp.Domain;
using UsersApp.Infrastructure.Authentication;

namespace UsersApp.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ApiClient> ApiClients { get; set; }
}