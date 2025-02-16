﻿using Microsoft.EntityFrameworkCore;
using UsersApp.Database;
using UsersApp.Infrastructure;

namespace UsersApp.Api.Authentication;

public class ApiKeyFilter(AppDbContext dbContext) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext efiContext, 
        EndpointFilterDelegate next)
    {
        if (!efiContext.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var apiKey))
        {
            return TypedResults.Unauthorized();
        }
        
        var apiClient = await dbContext.ApiClients.SingleOrDefaultAsync(client => client.Key == apiKey.SingleOrDefault());

        if (apiClient is null)
        {
            return TypedResults.Unauthorized();
        }
        
        efiContext.HttpContext.Items.Add("ClientName", apiClient.Name);
        efiContext.HttpContext.Request.Headers.Remove("X-API-KEY");
        
        return await next(efiContext);
    }
}