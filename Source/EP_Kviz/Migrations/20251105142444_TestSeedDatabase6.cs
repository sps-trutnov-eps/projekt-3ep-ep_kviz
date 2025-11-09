using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class TestSeedDatabase6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ddbecc5b-ba8a-43c0-a5b6-a415bf30b6e7", "AQAAAAIAAYagAAAAEPM8o+rjYD7ckhUCQSDU3abTUIRpLMvpENQJgrP39sjEsEO347FCtbW+YMxClQfk1Q==", "40a544f9-6f81-412f-8a27-0a61ebf15d15" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "155185f8-1ac8-425d-beb1-4614b1838f7f", "AQAAAAIAAYagAAAAED4SNxq0kts6BmYrv0nD34FE1frkxdSMUSj85VtOdY0I+ODKAvaUkb09TABPJTPkLw==", "a36aac32-bd2f-45b4-96b3-08cb348ad83e" });
        }
    }
}
