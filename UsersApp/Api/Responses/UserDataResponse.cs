namespace UsersApp.Api.Responses;

public record UserDataResponse(
    string Username,
    string Fullname,
    string Email,
    string MobileNumber,
    string Language,
    string Culture
    );