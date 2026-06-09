using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileOnboarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                schema: "auth",
                table: "UserProfiles",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProfileCompleted",
                schema: "auth",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                schema: "auth",
                table: "UserProfiles",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LocationUpdatedAt",
                schema: "auth",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                schema: "auth",
                table: "UserProfiles",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Photos",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterestedInGender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MinAge = table.Column<int>(type: "int", nullable: false),
                    MaxAge = table.Column<int>(type: "int", nullable: false),
                    MaxDistanceKm = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_Latitude_Longitude",
                schema: "auth",
                table: "UserProfiles",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId",
                schema: "auth",
                table: "Photos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId",
                schema: "auth",
                table: "UserPreferences",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photos",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserPreferences",
                schema: "auth");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_Latitude_Longitude",
                schema: "auth",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsProfileCompleted",
                schema: "auth",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "auth",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LocationUpdatedAt",
                schema: "auth",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "auth",
                table: "UserProfiles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                schema: "auth",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
