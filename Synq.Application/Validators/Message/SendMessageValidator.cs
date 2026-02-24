using FluentValidation;
using Synq.Application.Features.Message.SendMessage;

namespace Synq.Application.Validators.Message;

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
  public SendMessageValidator()
  {
    RuleFor(x => x.Id).NotNull().NotEmpty();
    RuleFor(x => x.Content).NotNull().NotEmpty();
    RuleFor(x => x.IsChat).NotNull();
  }
}
