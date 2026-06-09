using FluentValidation;
using SameMess.Application.DTOs.Matching;
using SameMess.Domain.Enums;

namespace SameMess.Application.Validators;

public class SwipeRequestValidator : AbstractValidator<SwipeRequestDto>
{
    public SwipeRequestValidator()
    {
        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithMessage("TargetUserId is required.");

        RuleFor(x => x.Action)
            .Must(a => SwipeAction.All.Contains(a))
            .WithMessage($"Action must be one of: {string.Join(", ", SwipeAction.All)}.");
    }
}
