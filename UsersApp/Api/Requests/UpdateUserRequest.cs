using System.ComponentModel.DataAnnotations;

namespace UsersApp.Api.Requests;

public record UpdateUserRequest(
    string Fullname,
    string Email,
    string MobileNumber,
    string Language,
    string Culture,
    string Password
    );