using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDrawToGameSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "d0b9472d-f30e-4809-b951-f392d3e0ffec", "b1eb1c93-c613-4b1f-86f3-a1820441444d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "9f2cabdd-ab3b-42c9-a9e5-224a9f6f206a", "0c9a6fcd-4486-493e-b202-ddb30a423cb7" });
        }
    }
}
