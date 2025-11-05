using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class TestSeedDatabase5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "155185f8-1ac8-425d-beb1-4614b1838f7f", "AQAAAAIAAYagAAAAED4SNxq0kts6BmYrv0nD34FE1frkxdSMUSj85VtOdY0I+ODKAvaUkb09TABPJTPkLw==", "a36aac32-bd2f-45b4-96b3-08cb348ad83e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cbb5858f-76ca-4c04-8f51-50c264a48310", "AQAAAAIAAYagAAAAENzGF+WCz7QyIXijFlHlC9Ont04iWrLyFLWpjQZW4y1IR8E9MatQdPKpzytA18jZBA==", "51e6e4a6-3ce6-42e7-9390-7f96279f8567" });
        }
    }
}
