using Microsoft.EntityFrameworkCore;
using UsersApp.Api.Authentication;
using UsersApp.Domain;

namespace UsersApp.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ApiClient> ApiClients { get; set; }
}  