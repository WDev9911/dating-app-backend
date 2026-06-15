using FluentValidation;
using SameMess.Application.DTOs.Safety;

namespace SameMess.Application.Validators;

public class SetupPinValidator : AbstractValidator<SetupPinDto>
{
    public SetupPinValidator()
    {
        RuleFor(x => x.Pin)
            .NotEmpty().WithMessage("PIN is required.")
            .Matches(@"^\d{4,6}$").WithMessage("PIN must be 4 to 6 digits.");
    }
}

public class CheckinValidator : AbstractValidator<CheckinDto>
{
    public CheckinValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(s => s is "safe" or "help").WithMessage("Status must be 'safe' or 'help'.");
    }
}
