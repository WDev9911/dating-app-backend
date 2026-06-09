using FluentValidation;
using SameMess.Application.DTOs.Preference;
using SameMess.Domain.Enums;

namespace SameMess.Application.Validators;

public class UpdatePreferenceValidator : AbstractValidator<UpdatePreferenceDto>
{
    public UpdatePreferenceValidator()
    {
        RuleFor(x => x.InterestedInGender)
            .Must(g => GenderPreference.All.Contains(g))
            .WithMessage($"InterestedInGender must be one of: {string.Join(", ", GenderPreference.All)}.");

        RuleFor(x => x.MinAge)
            .InclusiveBetween(18, 99).WithMessage("MinAge must be between 18 and 99.");

        RuleFor(x => x.MaxAge)
            .InclusiveBetween(18, 99).WithMessage("MaxAge must be between 18 and 99.");

        RuleFor(x => x)
            .Must(x => x.MinAge <= x.MaxAge)
            .WithName("MinAge")
            .WithMessage("MinAge must be less than or equal to MaxAge.");

        RuleFor(x => x.MaxDistanceKm)
            .InclusiveBetween(1, 500).WithMessage("MaxDistanceKm must be between 1 and 500.");
    }
}
