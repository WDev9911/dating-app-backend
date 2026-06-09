using FluentValidation;
using SameMess.Application.DTOs.Profile;
using SameMess.Domain.Enums;

namespace SameMess.Application.Validators;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
{
    private const int MinAgeYears = 18;

    public UpdateProfileValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters.");

        When(x => !string.IsNullOrEmpty(x.Gender), () =>
        {
            RuleFor(x => x.Gender)
                .Must(g => Gender.All.Contains(g))
                .WithMessage($"Gender must be one of: {string.Join(", ", Gender.All)}.");
        });

        When(x => x.DateOfBirth.HasValue, () =>
        {
            RuleFor(x => x.DateOfBirth)
                .Must(BeAtLeastMinAge)
                .WithMessage($"You must be at least {MinAgeYears} years old.");
        });

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters.");

        When(x => x.Height.HasValue, () =>
        {
            RuleFor(x => x.Height)
                .InclusiveBetween(100, 250).WithMessage("Height (cm) must be between 100 and 250.");
        });

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Location must not exceed 200 characters.");

        RuleFor(x => x.DatingGoal)
            .MaximumLength(100).WithMessage("Dating goal must not exceed 100 characters.");
    }

    private static bool BeAtLeastMinAge(DateOnly? dateOfBirth)
    {
        if (dateOfBirth is not { } dob) return false;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (dob > today) return false; // không cho ngày sinh ở tương lai

        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age)) age--;
        return age >= MinAgeYears;
    }
}
