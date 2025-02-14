using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using UsersApp.Api.Requests;
using UsersApp.Api.Responses;
using UsersApp.Infrastructure;
using UsersApp.Tests.EndPointTests.Helpers;

namespace UsersApp.Tests.EndPointTests;

public class EndPointTests(TestWebApplicationFactory<Program> factory)
    : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    [Fact]
    public async Task PostUserWithValidParameters()
    {
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<AppDbContext>();
            if (db != null && db.Users.Any())
            {
                db.Users.RemoveRange(db.Users);
                await db.SaveChangesAsync();
            }
        }
        
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY","12345");
        
        var createUserRequest = new CreateUserRequest(
            "username123",
            "Janez Novak",
            "janeznovak@test.com",
            "0038641582862",
            "si",
            "si_SI",
            "passwordHash123==");

        var response = await _httpClient.PostAsJsonAsync("/users/", createUserRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var userResponse = await _httpClient.GetFromJsonAsync<UserDataResponse>("/users/username123");

        Assert.NotNull(userResponse);

        Assert.Equal(userResponse.Username, createUserRequest.Username);
        Assert.Equal(userResponse.Fullname, createUserRequest.Fullname);
        Assert.Equal(userResponse.Email, createUserRequest.Email);
        Assert.Equal(userResponse.MobileNumber, createUserRequest.MobileNumber);
        Assert.Equal(userResponse.Language, createUserRequest.Language);
        Assert.Equal(userResponse.Culture, createUserRequest.Culture);
    }
}