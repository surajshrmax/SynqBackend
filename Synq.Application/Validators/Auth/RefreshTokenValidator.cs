using FluentValidation;
using Synq.Application.Features.Auth.RefreshToken;

namespace Synq.Application.Validators.Auth;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotNull().NotEmpty();
    }
}
