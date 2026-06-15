using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyAndConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyQuestCompletions",
                schema: "gamification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PeriodKey = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    XpAwarded = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyQuestCompletions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeetupProposals",
                schema: "chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProposedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetupProposals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NudgeDismissals",
                schema: "chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    NudgeCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NudgeDismissals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserXp",
                schema: "gamification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalXp = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserXp", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyQuestCompletions_UserId_QuestCode_PeriodKey",
                schema: "gamification",
                table: "DailyQuestCompletions",
                columns: new[] { "UserId", "QuestCode", "PeriodKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeetupProposals_ConversationId",
                schema: "chat",
                table: "MeetupProposals",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_NudgeDismissals_UserId_ConversationId_NudgeCode",
                schema: "chat",
                table: "NudgeDismissals",
                columns: new[] { "UserId", "ConversationId", "NudgeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserXp_UserId",
                schema: "gamification",
                table: "UserXp",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyQuestCompletions",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "MeetupProposals",
                schema: "chat");

            migrationBuilder.DropTable(
                name: "NudgeDismissals",
                schema: "chat");

            migrationBuilder.DropTable(
                name: "UserXp",
                schema: "gamification");
        }
    }
}
