using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AICommunicationService.RAG.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalFieldToDocumnetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsReadyToUse",
                table: "Documents",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "IsReadyToUse",
                table: "Documents");
        }
    }
}
