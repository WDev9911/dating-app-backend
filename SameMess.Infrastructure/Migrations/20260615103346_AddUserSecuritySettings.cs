using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSecuritySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LoginAlertsEnabled",
                schema: "auth",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                schema: "auth",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginAlertsEnabled",
                schema: "auth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                schema: "auth",
                table: "Users");
        }
    }
}
