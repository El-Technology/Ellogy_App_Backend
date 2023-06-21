using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCompexKeyToForgotPasswordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ForgotPassword",
                table: "ForgotPassword");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ForgotPassword");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "ForgotPassword",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForgotPassword",
                table: "ForgotPassword",
                columns: new[] { "UserId", "Token" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ForgotPassword",
                table: "ForgotPassword");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "ForgotPassword",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ForgotPassword",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForgotPassword",
                table: "ForgotPassword",
                column: "Id");
        }
    }
}
