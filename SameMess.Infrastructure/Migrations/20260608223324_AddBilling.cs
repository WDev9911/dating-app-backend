using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "billing");

            migrationBuilder.CreateTable(
                name: "PaymentOrders",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TxnRef = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlanCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AmountVnd = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VnpTransactionNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VnpResponseCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentOrders_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PriceVnd = table.Column<int>(type: "int", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "billing",
                table: "Plans",
                columns: new[] { "Id", "Code", "DurationDays", "IsActive", "Name", "PriceVnd" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), "Free", 0, true, "Free", 0 },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), "Plus", 30, true, "Plus", 59000 },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), "Gold", 30, true, "Gold", 129000 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_TxnRef",
                schema: "billing",
                table: "PaymentOrders",
                column: "TxnRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_UserId",
                schema: "billing",
                table: "PaymentOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_Code",
                schema: "billing",
                table: "Plans",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                schema: "billing",
                table: "Subscriptions",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentOrders",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "Plans",
                schema: "billing");

            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "billing");
        }
    }
}
