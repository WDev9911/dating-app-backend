using FluentValidation;
using SameMess.Application.DTOs.Safety;
using SameMess.Domain.Enums;

namespace SameMess.Application.Validators;

public class ReportRequestValidator : AbstractValidator<ReportRequestDto>
{
    public ReportRequestValidator()
    {
        RuleFor(x => x.Reason)
            .Must(r => ReportReason.All.Contains(r))
            .WithMessage($"Reason must be one of: {string.Join(", ", ReportReason.All)}.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }
}
