using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.DbContexts;

#nullable disable

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL.Migrations.Identity
{
    [DbContext(typeof(AdminIdentityDbContext))]
    [Migration("20260223162100_AddUserPasskeys")]
    public partial class AddUserPasskeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPasskeys",
                columns: table => new
                {
                    CredentialId = table.Column<byte[]>(type: "bytea", maxLength: 1024, nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasskeys", x => x.CredentialId);
                    table.ForeignKey(
                        name: "FK_UserPasskeys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPasskeys_UserId",
                table: "UserPasskeys",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPasskeys");
        }
    }
}
