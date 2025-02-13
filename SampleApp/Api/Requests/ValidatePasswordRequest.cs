using System.ComponentModel.DataAnnotations;

namespace SampleApp.Api.Requests;

public record ValidatePasswordRequest(
    [Required]
    string Password
    );