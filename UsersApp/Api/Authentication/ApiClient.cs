using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace UsersApp.Api.Authentication;

[Index(nameof(Key), IsUnique = true)]
public class ApiClient
{
    public required int Id { get; init; }
    [MaxLength(128)]
    public required string Key { get; init; }
    [MaxLength(100)]
    public required string Name { get; init; }
}