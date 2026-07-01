using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDatePassPayOs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuyerName",
                schema: "billing",
                table: "DatePassOrders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                schema: "billing",
                table: "DatePassOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerName",
                schema: "billing",
                table: "DatePassOrders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PayOsOrderCode",
                schema: "billing",
                table: "DatePassOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatePassOrders_PayOsOrderCode",
                schema: "billing",
                table: "DatePassOrders",
                column: "PayOsOrderCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DatePassOrders_PayOsOrderCode",
                schema: "billing",
                table: "DatePassOrders");

            migrationBuilder.DropColumn(
                name: "BuyerName",
                schema: "billing",
                table: "DatePassOrders");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                schema: "billing",
                table: "DatePassOrders");

            migrationBuilder.DropColumn(
                name: "PartnerName",
                schema: "billing",
                table: "DatePassOrders");

            migrationBuilder.DropColumn(
                name: "PayOsOrderCode",
                schema: "billing",
                table: "DatePassOrders");
        }
    }
}
