using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WPFTest.Migrations
{
    /// <inheritdoc />
    public partial class AddUserVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserAccounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "UserAccounts",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "UserAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "VerificationToken", "VerifiedAt" },
                values: new object[] { new DateTime(2025, 3, 9, 10, 45, 46, 307, DateTimeKind.Local).AddTicks(8274), "$2a$11$bNDvIyvA81fldmlEX/o2Ue7PTsiNz5ztI3kBwwO7IF1lWY8T5rfJa", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "UserAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserAccounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 9, 10, 37, 19, 908, DateTimeKind.Local).AddTicks(1443), "$2a$11$rZ60kMgN4yoAu7wURgMqLefVek0D/eQheU.die5B8iD9dVfGfKiWO" });
        }
    }
}
