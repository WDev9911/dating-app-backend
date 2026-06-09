using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSwipeAndMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "matching");

            migrationBuilder.CreateTable(
                name: "Matches",
                schema: "matching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserAId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserBId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Users_UserAId",
                        column: x => x.UserAId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Users_UserBId",
                        column: x => x.UserBId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Swipes",
                schema: "matching",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    SwiperId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Swipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Swipes_Users_SwiperId",
                        column: x => x.SwiperId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Swipes_Users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_UserAId_UserBId",
                schema: "matching",
                table: "Matches",
                columns: new[] { "UserAId", "UserBId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_UserBId",
                schema: "matching",
                table: "Matches",
                column: "UserBId");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwiperId_TargetUserId",
                schema: "matching",
                table: "Swipes",
                columns: new[] { "SwiperId", "TargetUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_TargetUserId",
                schema: "matching",
                table: "Swipes",
                column: "TargetUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches",
                schema: "matching");

            migrationBuilder.DropTable(
                name: "Swipes",
                schema: "matching");
        }
    }
}
