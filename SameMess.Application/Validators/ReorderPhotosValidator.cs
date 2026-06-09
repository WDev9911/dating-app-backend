using FluentValidation;
using SameMess.Application.DTOs.Profile;

namespace SameMess.Application.Validators;

public class ReorderPhotosValidator : AbstractValidator<ReorderPhotosDto>
{
    public ReorderPhotosValidator()
    {
        RuleFor(x => x.PhotoIds)
            .NotEmpty().WithMessage("PhotoIds must not be empty.");

        RuleForEach(x => x.PhotoIds)
            .NotEqual(Guid.Empty).WithMessage("PhotoIds must not contain empty GUIDs.");
    }
}
