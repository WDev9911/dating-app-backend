using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;
using SameMess.Infrastructure.Repositories;
using SameMess.Infrastructure.Services;
using SameMess.Infrastructure.Settings;

namespace SameMess.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.Configure<ResendSettings>(configuration.GetSection("Resend"));
        services.Configure<AiSettings>(configuration.GetSection("Ai"));
        services.Configure<VNPaySettings>(configuration.GetSection("VNPay"));
        services.Configure<WebPushSettings>(configuration.GetSection("WebPush"));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
        services.AddScoped<IDiscoveryRepository, DiscoveryRepository>();
        services.AddScoped<ISwipeRepository, SwipeRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IBlockRepository, BlockRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IMatchPlantRepository, MatchPlantRepository>();
        services.AddScoped<IUserInventoryRepository, UserInventoryRepository>();
        services.AddScoped<IUserTaskProgressRepository, UserTaskProgressRepository>();
        services.AddScoped<IReputationScoreRepository, ReputationScoreRepository>();
        services.AddScoped<IReputationEventRepository, ReputationEventRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPaymentOrderRepository, PaymentOrderRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IPushSubscriptionRepository, PushSubscriptionRepository>();
        services.AddScoped<IInterestRepository, InterestRepository>();
        services.AddScoped<IUserInterestRepository, UserInterestRepository>();
        services.AddScoped<ISearchRepository, SearchRepository>();

        services.AddScoped<ITokenService, TokenService>();

        // Có Resend:ApiKey -> gửi email qua Resend HTTP API (chạy được trên cloud, không bị chặn cổng SMTP).
        // Không có -> dùng SMTP (tiện cho local dev với Gmail).
        if (!string.IsNullOrWhiteSpace(configuration["Resend:ApiKey"]))
            services.AddHttpClient<IEmailService, ResendEmailService>(c => c.Timeout = TimeSpan.FromSeconds(20));
        else
            services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPaymentGateway, VNPayGateway>();
        services.AddScoped<IFaceVerificationService, StubFaceVerificationService>();
        services.AddScoped<IPushSender, LogPushSender>();

        return services;
    }
}
