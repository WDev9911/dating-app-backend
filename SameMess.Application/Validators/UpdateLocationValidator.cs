using FluentValidation;
using SameMess.Application.DTOs.Profile;

namespace SameMess.Application.Validators;

public class UpdateLocationValidator : AbstractValidator<UpdateLocationDto>
{
    public UpdateLocationValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
    }
}
