using System.Security.Cryptography;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SameMess.Application.DTOs.Auth;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IOtpCodeRepository _otpCodeRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    private const int RefreshTokenDays = 14;
    private const int OtpExpiryMinutes = 10;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IOtpCodeRepository otpCodeRepository,
        ITokenService tokenService,
        IEmailService emailService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _otpCodeRepository = otpCodeRepository;
        _tokenService = tokenService;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
            throw new ConflictException("Email already exists.");

        if (!string.IsNullOrEmpty(dto.PhoneNumber) && await _userRepository.PhoneExistsAsync(dto.PhoneNumber))
            throw new ConflictException("Phone number already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email.ToLower().Trim(),
            PhoneNumber = dto.PhoneNumber?.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IsEmailVerified = false,
            IsPhoneVerified = false,
            Role = UserRole.User,
            Status = UserStatus.PendingVerification,
            CreatedAt = DateTime.UtcNow,
            Profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                DisplayName = dto.DisplayName.Trim(),
                CreatedAt = DateTime.UtcNow,
            }
        };

        await _userRepository.AddAsync(user);
        await _otpCodeRepository.InvalidateAllOtpsForEmailAsync(dto.Email, OtpPurpose.EmailVerification);

        var otpCode = GenerateSecureOtp();
        await _otpCodeRepository.AddAsync(new OtpCode
        {
            Id = Guid.NewGuid(),
            Email = dto.Email.ToLower().Trim(),
            Code = otpCode,
            Purpose = OtpPurpose.EmailVerification,
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow,
        });

        await _userRepository.SaveChangesAsync();

        try
        {
            await _emailService.SendOtpEmailAsync(dto.Email, otpCode, OtpPurpose.EmailVerification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP to {Email}", dto.Email);
        }

        return new RegisterResponseDto
        {
            Message = "Registration successful. Please check your email for the 6-digit verification code.",
            Email = dto.Email,
        };
    }

    public async Task<AuthTokenResult> VerifyEmailAsync(VerifyOtpRequestDto dto)
    {
        var otp = await _otpCodeRepository.GetValidOtpAsync(
            dto.Email, dto.OtpCode, OtpPurpose.EmailVerification);

        if (otp == null)
            throw new UnauthorizedException("Invalid or expired OTP code.");

        var user = await _userRepository.GetByEmailAsync(dto.Email)
            ?? throw new NotFoundException("User", dto.Email);

        otp.IsUsed = true;
        user.IsEmailVerified = true;
        user.Status = UserStatus.Active;
        user.UpdatedAt = DateTime.UtcNow;

        await _otpCodeRepository.UpdateAsync(otp);
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return await BuildAuthTokenResultAsync(user, null, null);
    }

    public async Task<AuthTokenResult> LoginAsync(LoginRequestDto dto, string? ipAddress, string? userAgent)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        if (user.Status == UserStatus.Banned)
            throw new ForbiddenException("Your account has been banned.");

        if (user.Status == UserStatus.PendingVerification)
            throw new ForbiddenException("Please verify your email before logging in.");

        if (user.Status != UserStatus.Active)
            throw new ForbiddenException("Your account is inactive.");

        return await BuildAuthTokenResultAsync(user, ipAddress, userAgent);
    }

    public async Task<AuthTokenResult> RefreshTokenAsync(string plainRefreshToken, string? ipAddress, string? userAgent)
    {
        var tokenHash = _tokenService.HashToken(plainRefreshToken);
        var stored = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (stored == null || !stored.IsActive)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        var (newPlain, newHash) = _tokenService.GenerateRefreshToken();
        stored.RevokedAt = DateTime.UtcNow;
        stored.ReplacedByTokenHash = newHash;

        await _refreshTokenRepository.UpdateAsync(stored);
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = stored.UserId,
            TokenHash = newHash,
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            UserAgent = userAgent,
        });
        await _refreshTokenRepository.SaveChangesAsync();

        var accessToken = _tokenService.GenerateAccessToken(stored.User);
        return new AuthTokenResult
        {
            Response = new AuthResponseDto { AccessToken = accessToken, User = _mapper.Map<UserInfoDto>(stored.User) },
            PlainRefreshToken = newPlain,
        };
    }

    public async Task RevokeRefreshTokenAsync(string plainRefreshToken)
    {
        if (string.IsNullOrEmpty(plainRefreshToken)) return;

        var stored = await _refreshTokenRepository.GetByTokenHashAsync(_tokenService.HashToken(plainRefreshToken));
        if (stored is { IsActive: true })
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(stored);
            await _refreshTokenRepository.SaveChangesAsync();
        }
    }

    public async Task<UserInfoDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetWithProfileAsync(userId)
            ?? throw new NotFoundException("User", userId);

        return _mapper.Map<UserInfoDto>(user);
    }

    private async Task<AuthTokenResult> BuildAuthTokenResultAsync(User user, string? ipAddress, string? userAgent)
    {
        var (plainToken, tokenHash) = _tokenService.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            UserAgent = userAgent,
        });
        await _refreshTokenRepository.SaveChangesAsync();

        return new AuthTokenResult
        {
            Response = new AuthResponseDto
            {
                AccessToken = _tokenService.GenerateAccessToken(user),
                User = _mapper.Map<UserInfoDto>(user),
            },
            PlainRefreshToken = plainToken,
        };
    }

    private static string GenerateSecureOtp() =>
        RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
}
