using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBoost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BoostedUntil",
                schema: "auth",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoostedUntil",
                schema: "auth",
                table: "UserProfiles");
        }
    }
}
