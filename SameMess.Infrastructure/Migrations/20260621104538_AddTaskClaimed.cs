using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskClaimed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Claimed",
                schema: "gamification",
                table: "UserTaskProgress",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClaimedAt",
                schema: "gamification",
                table: "UserTaskProgress",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Claimed",
                schema: "gamification",
                table: "UserTaskProgress");

            migrationBuilder.DropColumn(
                name: "ClaimedAt",
                schema: "gamification",
                table: "UserTaskProgress");
        }
    }
}
