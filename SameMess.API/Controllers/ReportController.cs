using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
using SameMess.Application.DTOs.Safety;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api")]
public class ReportController : ApiControllerBase
{
    private readonly IReportService _reportService;
    private readonly IValidator<ReportRequestDto> _reportValidator;

    public ReportController(IReportService reportService, IValidator<ReportRequestDto> reportValidator)
    {
        _reportService = reportService;
        _reportValidator = reportValidator;
    }

    /// <summary>Báo cáo một người vì vi phạm.</summary>
    [HttpPost("users/{id:guid}/report")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Report(Guid id, [FromBody] ReportRequestDto dto)
    {
        var validation = await _reportValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        await _reportService.ReportAsync(CurrentUserId, id, dto);
        return NoContent();
    }
}
