using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class clearnutiDatabazedfg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "e67ad975-03a1-43f4-8573-a51f9cd9e870", "5c5bc609-bc7d-4be3-a030-43b4941a0902" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "f3208ed4-1ee2-45ff-b513-543f0c31c234", "f34568f8-96c4-4a05-96ad-962e4dbdffe8" });
        }
    }
}
