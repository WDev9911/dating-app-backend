using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
using SameMess.Application.DTOs.Auth;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json", "application/problem+json")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";
    private const int RefreshTokenDays = 14;

    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IValidator<VerifyOtpRequestDto> _verifyOtpValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterRequestDto> registerValidator,
        IValidator<LoginRequestDto> loginValidator,
        IValidator<VerifyOtpRequestDto> verifyOtpValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _verifyOtpValidator = verifyOtpValidator;
    }

    /// <summary>Register a new account. A 6-digit OTP is sent to the provided email.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var validation = await _registerValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        var result = await _authService.RegisterAsync(dto);
        return Ok(result);
    }

    /// <summary>Verify email with the OTP received after registration.</summary>
    [HttpPost("verify-email")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyOtpRequestDto dto)
    {
        var validation = await _verifyOtpValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        var result = await _authService.VerifyEmailAsync(dto);
        SetRefreshTokenCookie(result.PlainRefreshToken);
        return Ok(result.Response);
    }

    /// <summary>Login with email and password. Sets an HttpOnly refresh token cookie.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var validation = await _loginValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();
        var result = await _authService.LoginAsync(dto, ip, ua);
        SetRefreshTokenCookie(result.PlainRefreshToken);
        return Ok(result.Response);
    }

    /// <summary>Use the refresh token cookie to get a new access token. Rotates the refresh token.</summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken()
    {
        var token = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrEmpty(token))
            return Problem(title: "Unauthorized", detail: "No refresh token provided.",
                statusCode: StatusCodes.Status401Unauthorized);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();
        var result = await _authService.RefreshTokenAsync(token, ip, ua);
        SetRefreshTokenCookie(result.PlainRefreshToken);
        return Ok(result.Response);
    }

    /// <summary>Logout — revokes the refresh token and clears the cookie.</summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Cookies[RefreshTokenCookieName];
        if (!string.IsNullOrEmpty(token))
            await _authService.RevokeRefreshTokenAsync(token);

        ClearRefreshTokenCookie();
        return Ok(new { message = "Logged out successfully." });
    }

    /// <summary>Get current authenticated user info.</summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe()
    {
        var sub = User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(sub, out var userId))
            return Problem(title: "Unauthorized", detail: "Invalid token claims.",
                statusCode: StatusCodes.Status401Unauthorized);

        var result = await _authService.GetCurrentUserAsync(userId);
        return Ok(result);
    }

    /// <summary>Xoá vĩnh viễn tài khoản hiện tại + toàn bộ dữ liệu liên quan (banned tự xoá khi thoát).</summary>
    [Authorize]
    [HttpDelete("account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAccount()
    {
        var sub = User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(sub, out var userId))
            return Problem(title: "Unauthorized", detail: "Invalid token claims.",
                statusCode: StatusCodes.Status401Unauthorized);

        await _authService.DeleteAccountAsync(userId);
        ClearRefreshTokenCookie();
        return Ok(new { message = "Account deleted." });
    }

    private void SetRefreshTokenCookie(string token)
    {
        // SameSite=None is required because the frontend (e.g. localhost:5173
        // or the Vercel deployment) runs on a different origin than the API,
        // making every request cross-site. Lax cookies are not attached to
        // cross-site POST requests, so /api/auth/refresh would never see the
        // cookie and the user would be logged out on every page reload.
        Response.Cookies.Append(RefreshTokenCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(RefreshTokenDays),
        });
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Append(RefreshTokenCookieName, string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
        });
    }
}
