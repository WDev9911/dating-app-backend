using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
using SameMess.Application.DTOs.Profile;
using SameMess.Application.DTOs.Verification;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/profile")]
public class ProfileController : ApiControllerBase
{
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };
    private const long MaxFileBytes = 5 * 1024 * 1024; // 5MB

    private readonly IProfileService _profileService;
    private readonly IProfileVerificationService _verificationService;
    private readonly IValidator<UpdateProfileDto> _updateProfileValidator;
    private readonly IValidator<UpdateLocationDto> _updateLocationValidator;
    private readonly IValidator<ReorderPhotosDto> _reorderPhotosValidator;

    public ProfileController(
        IProfileService profileService,
        IProfileVerificationService verificationService,
        IValidator<UpdateProfileDto> updateProfileValidator,
        IValidator<UpdateLocationDto> updateLocationValidator,
        IValidator<ReorderPhotosDto> reorderPhotosValidator)
    {
        _profileService = profileService;
        _verificationService = verificationService;
        _updateProfileValidator = updateProfileValidator;
        _updateLocationValidator = updateLocationValidator;
        _reorderPhotosValidator = reorderPhotosValidator;
    }

    /// <summary>Lấy hồ sơ đầy đủ của tôi (kèm ảnh và preferences).</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProfile()
        => Ok(await _profileService.GetMyProfileAsync(CurrentUserId));

    /// <summary>Cập nhật thông tin hồ sơ (tên, giới tính, ngày sinh, bio, mục tiêu...).</summary>
    [HttpPut]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var validation = await _updateProfileValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        return Ok(await _profileService.UpdateProfileAsync(CurrentUserId, dto));
    }

    /// <summary>Cập nhật tọa độ (client web lấy qua navigator.geolocation).</summary>
    [HttpPut("location")]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto dto)
    {
        var validation = await _updateLocationValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        return Ok(await _profileService.UpdateLocationAsync(CurrentUserId, dto));
    }

    /// <summary>Upload một ảnh (multipart/form-data, field "file"). Tối đa 5MB, định dạng jpg/png/webp.</summary>
    [HttpPost("photos")]
    [ProducesResponseType(typeof(PhotoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UploadPhoto(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Problem(title: "Bad Request", detail: "No file uploaded.",
                statusCode: StatusCodes.Status400BadRequest);

        if (file.Length > MaxFileBytes)
            return Problem(title: "Bad Request", detail: "File must not exceed 5MB.",
                statusCode: StatusCodes.Status400BadRequest);

        if (!AllowedContentTypes.Contains(file.ContentType))
            return Problem(title: "Bad Request", detail: "Only jpg, png or webp images are allowed.",
                statusCode: StatusCodes.Status400BadRequest);

        await using var stream = file.OpenReadStream();
        var photo = await _profileService.AddPhotoAsync(
            CurrentUserId, stream, file.FileName, file.ContentType);
        return Ok(photo);
    }

    /// <summary>Xóa một ảnh.</summary>
    [HttpDelete("photos/{photoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePhoto(Guid photoId)
    {
        await _profileService.DeletePhotoAsync(CurrentUserId, photoId);
        return NoContent();
    }

    /// <summary>Đặt một ảnh làm ảnh đại diện chính.</summary>
    [HttpPut("photos/{photoId:guid}/primary")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPrimaryPhoto(Guid photoId)
    {
        await _profileService.SetPrimaryPhotoAsync(CurrentUserId, photoId);
        return NoContent();
    }

    /// <summary>Gửi selfie để xác minh khuôn mặt (multipart, field "file"). So khớp với ảnh hồ sơ → tự duyệt / chờ admin.</summary>
    [HttpPost("verify-face")]
    [ProducesResponseType(typeof(VerificationStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> VerifyFace(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Problem(title: "Bad Request", detail: "No file uploaded.",
                statusCode: StatusCodes.Status400BadRequest);

        if (file.Length > MaxFileBytes)
            return Problem(title: "Bad Request", detail: "File must not exceed 5MB.",
                statusCode: StatusCodes.Status400BadRequest);

        if (!AllowedContentTypes.Contains(file.ContentType))
            return Problem(title: "Bad Request", detail: "Only jpg, png or webp images are allowed.",
                statusCode: StatusCodes.Status400BadRequest);

        await using var stream = file.OpenReadStream();
        var result = await _verificationService.SubmitAsync(
            CurrentUserId, stream, file.FileName, file.ContentType);
        return Ok(result);
    }

    /// <summary>Trạng thái xác minh khuôn mặt của tôi.</summary>
    [HttpGet("verification")]
    [ProducesResponseType(typeof(VerificationStatusDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVerificationStatus()
        => Ok(await _verificationService.GetMyStatusAsync(CurrentUserId));

    /// <summary>Kích hoạt Boost: đẩy hồ sơ lên đầu feed người khác trong 30 phút.</summary>
    [HttpPost("boost")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Boost()
    {
        var boostedUntil = await _profileService.BoostAsync(CurrentUserId);
        return Ok(new { boostedUntil });
    }

    /// <summary>Sắp xếp lại thứ tự hiển thị ảnh (gửi đủ id của tất cả ảnh theo thứ tự mới).</summary>
    [HttpPut("photos/order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReorderPhotos([FromBody] ReorderPhotosDto dto)
    {
        var validation = await _reorderPhotosValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        await _profileService.ReorderPhotosAsync(CurrentUserId, dto);
        return NoContent();
    }
}
