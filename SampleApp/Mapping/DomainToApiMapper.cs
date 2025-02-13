using SampleApp.Api.Responses;
using SampleApp.Domain;

namespace SampleApp.Mapping;

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