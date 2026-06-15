using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SameMess.Application.Interfaces.Services;
using SameMess.Application.Mappings;
using SameMess.Application.Services;

namespace SameMess.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AuthMappingProfile).Assembly);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IPreferenceService, PreferenceService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IInterestsService, InterestsService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<ISafetyService, SafetyService>();
        services.AddScoped<IDiscoveryService, DiscoveryService>();
        services.AddScoped<ISwipeService, SwipeService>();
        services.AddScoped<IMatchService, MatchService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IIcebreakerService, IcebreakerService>();
        services.AddScoped<IBlockService, BlockService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAdminReportService, AdminReportService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IPlantService, PlantService>();
        services.AddScoped<IReputationService, ReputationService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IProfileVerificationService, ProfileVerificationService>();
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
}
