using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class TestSeedDatabase10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "88fa1547-df41-45fd-8d3d-9266c07e2ba0", "fd21fb12-f4a5-402a-9d90-ce2af2301679" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "3c4d5e6f-7g8h-9i0j-1k2l-3m4n5o6p7q8r", "2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q" });
        }
    }
}
