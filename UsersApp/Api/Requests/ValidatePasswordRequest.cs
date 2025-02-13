using System.ComponentModel.DataAnnotations;

namespace UsersApp.Api.Requests;

public record ValidatePasswordRequest(
    [Required]
    string Password
    );