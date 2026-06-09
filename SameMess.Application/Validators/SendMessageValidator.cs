using FluentValidation;
using SameMess.Application.DTOs.Chat;

namespace SameMess.Application.Validators;

public class SendMessageValidator : AbstractValidator<SendMessageDto>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content is required.")
            .MaximumLength(2000).WithMessage("Message must not exceed 2000 characters.");
    }
}
