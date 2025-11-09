using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class TestSeedDatabase9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJ8SHVpTXGhxT+OLvVlCJHNvbKGJWQvnNzVaLkOqBfJQXZBGHgBbU0qU9zZ3lA3fxA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDWyVM0n1gyN1kSrNYJ4j6T0l9KR8VP0koNrYKA1MngcJMB7yWcA+OmK+8/ORlLPSA==");
        }
    }
}
