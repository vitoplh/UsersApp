using System.ComponentModel.DataAnnotations;

namespace SampleApp.Api.Requests;

public record CreateUserRequest(
    [Required]
    string Username,
    [Required]
    string Fullname,
    [Required]
    string Email,
    [Required]
    string MobileNumber,
    [Required]
    string Language,
    [Required]
    string Culture,
    [Required]
    string Password
    );