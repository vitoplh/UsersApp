using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SampleApp.Domain;

[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int? Id { get; init; }
    [MaxLength(30)]
    public string Username {get; set;}
    [MaxLength(200)]
    public string Fullname {get; set;}
    [MaxLength(200)]
    public string Email {get; set;}
    [MaxLength(20)]
    public string MobileNumber {get; set;}
    [MaxLength(2)]
    public string Language {get; set;}
    [MaxLength(5)]
    public string Culture {get; set;}
    [MaxLength(128)]
    public string PasswordHash {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    
    public bool IsDeleted {get; set;}
}