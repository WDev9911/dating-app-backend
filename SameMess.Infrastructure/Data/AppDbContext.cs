using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<UserPreference> UserPreferences { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Swipe> Swipes { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Block> Blocks { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }
    public DbSet<MatchPlant> MatchPlants { get; set; }
    public DbSet<UserInventory> UserInventories { get; set; }
    public DbSet<UserTaskProgress> UserTaskProgress { get; set; }
    public DbSet<ReputationScore> ReputationScores { get; set; }
    public DbSet<ReputationEvent> ReputationEvents { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<PaymentOrder> PaymentOrders { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PushSubscription> PushSubscriptions { get; set; }
    public DbSet<Interest> Interests { get; set; }
    public DbSet<UserInterest> UserInterests { get; set; }
    public DbSet<SafetyProfile> SafetyProfiles { get; set; }
    public DbSet<EmergencyContact> EmergencyContacts { get; set; }
    public DbSet<SafetyCheckIn> SafetyCheckIns { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<AdminNote> AdminNotes { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<AppSetting> AppSettings { get; set; }
    public DbSet<FeatureFlag> FeatureFlags { get; set; }
    public DbSet<UserXp> UserXp { get; set; }
    public DbSet<DailyQuestCompletion> DailyQuestCompletions { get; set; }
    public DbSet<NudgeDismissal> NudgeDismissals { get; set; }
    public DbSet<MeetupProposal> MeetupProposals { get; set; }
    public DbSet<Venue> Venues { get; set; }
    public DbSet<VenueCombo> VenueCombos { get; set; }
    public DbSet<DatePassOrder> DatePassOrders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("auth");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
