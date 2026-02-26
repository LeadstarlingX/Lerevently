using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lerevently.Modules.Users.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_Roles_Permessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "users",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "users",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                schema: "users",
                columns: table => new
                {
                    PermissionCode = table.Column<string>(type: "character varying(100)", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => new { x.PermissionCode, x.RoleName });
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_PermissionCode",
                        column: x => x.PermissionCode,
                        principalSchema: "users",
                        principalTable: "permissions",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_RoleName",
                        column: x => x.RoleName,
                        principalSchema: "users",
                        principalTable: "roles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                schema: "users",
                columns: table => new
                {
                    role_name = table.Column<string>(type: "character varying(50)", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.role_name, x.UserId });
                    table.ForeignKey(
                        name: "FK_user_roles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "users",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_name",
                        column: x => x.role_name,
                        principalSchema: "users",
                        principalTable: "roles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "users",
                table: "permissions",
                column: "Code",
                values: new object[]
                {
                    "carts:add",
                    "carts:read",
                    "carts:remove",
                    "categories:read",
                    "categories:update",
                    "event-statistics:read",
                    "events:read",
                    "events:search",
                    "events:update",
                    "orders:create",
                    "orders:read",
                    "ticket-types:read",
                    "ticket-types:update",
                    "tickets:check-in",
                    "tickets:read",
                    "users:read",
                    "users:update"
                });

            migrationBuilder.InsertData(
                schema: "users",
                table: "roles",
                column: "Name",
                values: new object[]
                {
                    "Administrator",
                    "Member"
                });

            migrationBuilder.InsertData(
                schema: "users",
                table: "role_permissions",
                columns: new[] { "PermissionCode", "RoleName" },
                values: new object[,]
                {
                    { "carts:add", "Administrator" },
                    { "carts:add", "Member" },
                    { "carts:read", "Administrator" },
                    { "carts:read", "Member" },
                    { "carts:remove", "Administrator" },
                    { "carts:remove", "Member" },
                    { "categories:read", "Administrator" },
                    { "categories:update", "Administrator" },
                    { "event-statistics:read", "Administrator" },
                    { "events:read", "Administrator" },
                    { "events:search", "Administrator" },
                    { "events:search", "Member" },
                    { "events:update", "Administrator" },
                    { "orders:create", "Administrator" },
                    { "orders:create", "Member" },
                    { "orders:read", "Administrator" },
                    { "orders:read", "Member" },
                    { "ticket-types:read", "Administrator" },
                    { "ticket-types:read", "Member" },
                    { "ticket-types:update", "Administrator" },
                    { "tickets:check-in", "Administrator" },
                    { "tickets:check-in", "Member" },
                    { "tickets:read", "Administrator" },
                    { "tickets:read", "Member" },
                    { "users:read", "Administrator" },
                    { "users:read", "Member" },
                    { "users:update", "Administrator" },
                    { "users:update", "Member" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_RoleName",
                schema: "users",
                table: "role_permissions",
                column: "RoleName");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_UserId",
                schema: "users",
                table: "user_roles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_permissions",
                schema: "users");

            migrationBuilder.DropTable(
                name: "user_roles",
                schema: "users");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "users");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "users");
        }
    }
}
