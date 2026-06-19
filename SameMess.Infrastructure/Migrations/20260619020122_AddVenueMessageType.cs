using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVenueMessageType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "chat",
                table: "Messages",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "VenueId",
                schema: "chat",
                table: "Messages",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "chat",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "VenueId",
                schema: "chat",
                table: "Messages");
        }
    }
}
