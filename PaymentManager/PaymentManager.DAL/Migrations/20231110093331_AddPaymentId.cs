using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Payments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentId");
        }
    }
}
