using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class TestSeeduDatabáze : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1", 0, "9b112122-1caa-4a84-83dd-c289b3bb7cbe", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "", null, false, "02f5b8d5-da55-44bf-a1a2-7b022fef9531", false, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");
        }
    }
}
