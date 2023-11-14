using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentProductId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "AmountOfPoints",
                table: "Payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountOfPoints",
                table: "Payments");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
