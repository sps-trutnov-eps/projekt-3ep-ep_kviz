using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class StatistikyHrani : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PocetOdehranychHer",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PocetVyhranychHer",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "PocetOdehranychHer", "PocetVyhranychHer", "SecurityStamp" },
                values: new object[] { "9f2cabdd-ab3b-42c9-a9e5-224a9f6f206a", 0, 0, "0c9a6fcd-4486-493e-b202-ddb30a423cb7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PocetOdehranychHer",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PocetVyhranychHer",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "e67ad975-03a1-43f4-8573-a51f9cd9e870", "5c5bc609-bc7d-4be3-a030-43b4941a0902" });
        }
    }
}
