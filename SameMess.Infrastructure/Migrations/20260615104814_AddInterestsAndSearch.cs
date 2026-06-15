using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInterestsAndSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interests",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GroupName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInterests",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    InterestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInterests_Interests_InterestId",
                        column: x => x.InterestId,
                        principalSchema: "auth",
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInterests_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "Interests",
                columns: new[] { "Id", "CreatedAt", "GroupName", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("b0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lối sống", true, "Du lịch" },
                    { new Guid("b0000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lối sống", true, "Ẩm thực" },
                    { new Guid("b0000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lối sống", true, "Cà phê" },
                    { new Guid("b0000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lối sống", true, "Nấu ăn" },
                    { new Guid("b0000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lối sống", true, "Thú cưng" },
                    { new Guid("b0000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Giải trí", true, "Phim ảnh" },
                    { new Guid("b0000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Giải trí", true, "Âm nhạc" },
                    { new Guid("b0000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Giải trí", true, "Game" },
                    { new Guid("b0000000-0000-0000-0000-000000000009"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Giải trí", true, "Đọc sách" },
                    { new Guid("b0000000-0000-0000-0000-000000000010"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Giải trí", true, "Nhiếp ảnh" },
                    { new Guid("b0000000-0000-0000-0000-000000000011"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thể thao", true, "Gym" },
                    { new Guid("b0000000-0000-0000-0000-000000000012"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thể thao", true, "Yoga" },
                    { new Guid("b0000000-0000-0000-0000-000000000013"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thể thao", true, "Chạy bộ" },
                    { new Guid("b0000000-0000-0000-0000-000000000014"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thể thao", true, "Bơi lội" },
                    { new Guid("b0000000-0000-0000-0000-000000000015"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thể thao", true, "Leo núi" },
                    { new Guid("b0000000-0000-0000-0000-000000000016"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sáng tạo", true, "Vẽ" },
                    { new Guid("b0000000-0000-0000-0000-000000000017"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sáng tạo", true, "Thời trang" },
                    { new Guid("b0000000-0000-0000-0000-000000000018"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sáng tạo", true, "Khiêu vũ" },
                    { new Guid("b0000000-0000-0000-0000-000000000019"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sáng tạo", true, "Công nghệ" },
                    { new Guid("b0000000-0000-0000-0000-000000000020"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lối sống", true, "Cắm trại" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Interests_Name",
                schema: "auth",
                table: "Interests",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInterests_InterestId",
                schema: "auth",
                table: "UserInterests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterests_UserId_InterestId",
                schema: "auth",
                table: "UserInterests",
                columns: new[] { "UserId", "InterestId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInterests",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Interests",
                schema: "auth");
        }
    }
}
