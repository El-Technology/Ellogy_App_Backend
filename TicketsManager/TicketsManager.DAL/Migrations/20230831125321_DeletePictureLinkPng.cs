using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeletePictureLinkPng : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureLinkPng",
                table: "TicketDiagrams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureLinkPng",
                table: "TicketDiagrams",
                type: "text",
                nullable: true);
        }
    }
}
