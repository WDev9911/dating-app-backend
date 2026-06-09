using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
using SameMess.Application.DTOs.Gamification;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/plants")]
public class PlantsController : ApiControllerBase
{
    private readonly IPlantService _plantService;
    private readonly IValidator<WaterRequestDto> _waterValidator;

    public PlantsController(IPlantService plantService, IValidator<WaterRequestDto> waterValidator)
    {
        _plantService = plantService;
        _waterValidator = waterValidator;
    }

    /// <summary>Xem "Cây tình yêu" chung của một match (level, %, chuỗi). Tạo mới nếu chưa có.</summary>
    [HttpGet("{matchId:guid}")]
    [ProducesResponseType(typeof(PlantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlant(Guid matchId)
        => Ok(await _plantService.GetPlantAsync(CurrentUserId, matchId));

    /// <summary>Tưới cây bằng một nguyên liệu (Water/Sun/Fertilizer): tiêu kho → cộng % → check level + chuỗi.</summary>
    [HttpPost("{matchId:guid}/water")]
    [ProducesResponseType(typeof(WaterResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Water(Guid matchId, [FromBody] WaterRequestDto dto)
    {
        var validation = await _waterValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        return Ok(await _plantService.WaterAsync(CurrentUserId, matchId, dto.Material));
    }
}
