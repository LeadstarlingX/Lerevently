using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lerevently.Modules.Events.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Adding_Ticker_Categories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "events",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketTypes",
                schema: "events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTypes_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "events",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_CategoryId",
                schema: "events",
                table: "Events",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_EventId",
                schema: "events",
                table: "TicketTypes",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Categories_CategoryId",
                schema: "events",
                table: "Events",
                column: "CategoryId",
                principalSchema: "events",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Categories_CategoryId",
                schema: "events",
                table: "Events");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "events");

            migrationBuilder.DropTable(
                name: "TicketTypes",
                schema: "events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CategoryId",
                schema: "events",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "events",
                table: "Events");
        }
    }
}
