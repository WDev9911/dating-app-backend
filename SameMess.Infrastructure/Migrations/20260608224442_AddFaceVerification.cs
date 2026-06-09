using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SameMess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFaceVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPhotoVerified",
                schema: "auth",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerificationSelfieUrl",
                schema: "auth",
                table: "UserProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                schema: "auth",
                table: "UserProfiles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "None");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPhotoVerified",
                schema: "auth",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationSelfieUrl",
                schema: "auth",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                schema: "auth",
                table: "UserProfiles");
        }
    }
}
