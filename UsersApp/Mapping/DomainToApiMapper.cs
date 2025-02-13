using UsersApp.Api.Responses;
using UsersApp.Domain;

namespace UsersApp.Mapping;

public static class DomainToApiMapper
{
    public static UserDataResponse ToUserResponse(this User user)
    {
        return new UserDataResponse(
            user.Username,
            user.Fullname,
            user.Email,
            user.MobileNumber,
            user.Language,
            user.Culture);
    }
}