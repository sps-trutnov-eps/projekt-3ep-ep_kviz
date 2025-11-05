using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class TestSeedDatabase4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p", 0, "cbb5858f-76ca-4c04-8f51-50c264a48310", "test@epkviz.cz", true, false, null, "TEST@EPKVIZ.CZ", "TEST@EPKVIZ.CZ", "AQAAAAIAAYagAAAAENzGF+WCz7QyIXijFlHlC9Ont04iWrLyFLWpjQZW4y1IR8E9MatQdPKpzytA18jZBA==", null, false, "51e6e4a6-3ce6-42e7-9390-7f96279f8567", false, "test@epkviz.cz" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p");
        }
    }
}
