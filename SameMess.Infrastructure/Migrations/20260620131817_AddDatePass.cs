using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDatePass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatePassOrders",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    ComboId = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ComboTitle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AmountVnd = table.Column<int>(type: "integer", nullable: false),
                    CommissionVnd = table.Column<int>(type: "integer", nullable: false),
                    VoucherCode = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    PaidAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RedeemedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatePassOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VenueCombos",
                schema: "chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OriginalPriceVnd = table.Column<int>(type: "integer", nullable: false),
                    SalePriceVnd = table.Column<int>(type: "integer", nullable: false),
                    CommissionPercent = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueCombos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueCombos_Venues_VenueId",
                        column: x => x.VenueId,
                        principalSchema: "chat",
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatePassOrders_MatchId",
                schema: "billing",
                table: "DatePassOrders",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_DatePassOrders_VoucherCode",
                schema: "billing",
                table: "DatePassOrders",
                column: "VoucherCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VenueCombos_VenueId_IsActive",
                schema: "chat",
                table: "VenueCombos",
                columns: new[] { "VenueId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatePassOrders",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "VenueCombos",
                schema: "chat");
        }
    }
}
