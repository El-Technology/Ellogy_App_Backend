﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                 name: "Tickets",
                 columns: table => new
                 {
                     Id = table.Column<Guid>(type: "uuid", nullable: false),
                     Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                     Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                     Summary = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                     Comment = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                     CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                     UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                     Status = table.Column<int>(type: "integer", nullable: false),
                     UserId = table.Column<Guid>(type: "uuid", nullable: false)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Tickets", x => x.Id);
                     table.ForeignKey(
                         name: "FK_Tickets_Users_UserId",
                         column: x => x.UserId,
                         principalTable: "Users",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Cascade);
                 });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Sender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TicketId",
                table: "Messages",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
