using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGamification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gamification");

            migrationBuilder.CreateTable(
                name: "MatchPlants",
                schema: "gamification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    GrowthPercent = table.Column<int>(type: "int", nullable: false),
                    StreakCount = table.Column<int>(type: "int", nullable: false),
                    StreakDate = table.Column<DateOnly>(type: "date", nullable: true),
                    WaterDate = table.Column<DateOnly>(type: "date", nullable: true),
                    WateredByA = table.Column<bool>(type: "bit", nullable: false),
                    WateredByB = table.Column<bool>(type: "bit", nullable: false),
                    FreezeUsedWeekKey = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchPlants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchPlants_Matches_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "matching",
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInventories",
                schema: "gamification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInventories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTaskProgress",
                schema: "gamification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PeriodKey = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Progress = table.Column<int>(type: "int", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTaskProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTaskProgress_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchPlants_MatchId",
                schema: "gamification",
                table: "MatchPlants",
                column: "MatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInventories_UserId_MaterialType",
                schema: "gamification",
                table: "UserInventories",
                columns: new[] { "UserId", "MaterialType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTaskProgress_UserId_TaskCode_PeriodKey",
                schema: "gamification",
                table: "UserTaskProgress",
                columns: new[] { "UserId", "TaskCode", "PeriodKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchPlants",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "UserInventories",
                schema: "gamification");

            migrationBuilder.DropTable(
                name: "UserTaskProgress",
                schema: "gamification");
        }
    }
}
