using FluentValidation;
using UsersApp.Api.Requests;

namespace UsersApp.Api.Validation;

public class UpdateUserRequestvalidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestvalidator()
    {
        RuleFor(request => request.Fullname).NotEmpty().WithMessage("Fullname is required");
        RuleFor(request => request.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");
        // add regex
        RuleFor(request => request.MobileNumber).NotEmpty().WithMessage("MobileNumber is required");
        RuleFor(request => request.Language).NotEmpty().WithMessage("Language is required");
        RuleFor(request => request.Culture).NotEmpty().WithMessage("Culture is required");
        // add rules and fix the model with the same validation
        RuleFor(request => request.Password).NotEmpty().WithMessage("Password is required");
    }
}