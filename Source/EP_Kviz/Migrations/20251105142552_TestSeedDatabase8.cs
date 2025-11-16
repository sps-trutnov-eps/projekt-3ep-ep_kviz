using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class TestSeedDatabase8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3c4d5e6f-7g8h-9i0j-1k2l-3m4n5o6p7q8r", "AQAAAAIAAYagAAAAEDWyVM0n1gyN1kSrNYJ4j6T0l9KR8VP0koNrYKA1MngcJMB7yWcA+OmK+8/ORlLPSA==", "2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "283a1202-5d26-4543-9efc-c148ce13f405", "AQAAAAIAAYagAAAAEDuOCZmStsr8TcxJ03dVXSGUVuRu0GuIVa3sO8Ch6yRZzdYsIywds7MF/ifyjw0i8w==", "e023d81a-3bc3-4a83-a36f-ed9e5ad544db" });
        }
    }
}
