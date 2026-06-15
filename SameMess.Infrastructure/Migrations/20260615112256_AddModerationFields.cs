using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModerationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToAdminId",
                schema: "safety",
                table: "Reports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionNote",
                schema: "safety",
                table: "Reports",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                schema: "auth",
                table: "Photos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "auth",
                table: "Photos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Approved");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_Status",
                schema: "auth",
                table: "Photos",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Photos_Status",
                schema: "auth",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "AssignedToAdminId",
                schema: "safety",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ResolutionNote",
                schema: "safety",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                schema: "auth",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "auth",
                table: "Photos");
        }
    }
}
