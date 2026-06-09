using FluentValidation;
using SameMess.Application.DTOs.Gamification;
using SameMess.Domain.Enums;

namespace SameMess.Application.Validators;

public class WaterRequestValidator : AbstractValidator<WaterRequestDto>
{
    public WaterRequestValidator()
    {
        RuleFor(x => x.Material)
            .Must(m => PlantMaterial.All.Contains(m))
            .WithMessage($"Material must be one of: {string.Join(", ", PlantMaterial.All)}.");
    }
}
