using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class TestSeedDatabase7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "283a1202-5d26-4543-9efc-c148ce13f405", "AQAAAAIAAYagAAAAEDuOCZmStsr8TcxJ03dVXSGUVuRu0GuIVa3sO8Ch6yRZzdYsIywds7MF/ifyjw0i8w==", "e023d81a-3bc3-4a83-a36f-ed9e5ad544db" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ddbecc5b-ba8a-43c0-a5b6-a415bf30b6e7", "AQAAAAIAAYagAAAAEPM8o+rjYD7ckhUCQSDU3abTUIRpLMvpENQJgrP39sjEsEO347FCtbW+YMxClQfk1Q==", "40a544f9-6f81-412f-8a27-0a61ebf15d15" });
        }
    }
}
