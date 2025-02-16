using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UsersApp.Api.Authentication;
using UsersApp.Database;
using UsersApp.Infrastructure;

namespace UsersApp.Tests.EndPointTests.Helpers;

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite("Data Source=WebMinRouteGroup_tests.db")
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
                    });
            });
        });

        return base.CreateHost(builder);
    }
}