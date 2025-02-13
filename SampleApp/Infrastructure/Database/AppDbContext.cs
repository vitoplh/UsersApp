using Microsoft.EntityFrameworkCore;
using SampleApp.Domain;
using SampleApp.Infrastructure.Authentication;

namespace SampleApp.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ApiClient> ApiClients { get; set; }
}