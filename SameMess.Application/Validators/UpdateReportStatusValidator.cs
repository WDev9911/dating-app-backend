using FluentValidation;
using SameMess.Application.DTOs.Safety;
using SameMess.Domain.Enums;

namespace SameMess.Application.Validators;

public class UpdateReportStatusValidator : AbstractValidator<UpdateReportStatusDto>
{
    private static readonly string[] Allowed =
        { ReportStatus.Reviewed, ReportStatus.Resolved, ReportStatus.Dismissed };

    public UpdateReportStatusValidator()
    {
        RuleFor(x => x.Status)
            .Must(s => Allowed.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", Allowed)}.");
    }
}
