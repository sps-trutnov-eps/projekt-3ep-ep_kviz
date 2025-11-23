using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP_Kviz.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreboardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    ElapsedMs = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GameMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "35e20f93-dcbd-4595-92f5-d70bad3146db", "7b858026-8fb1-43df-a214-efc6f59294a2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "e67ad975-03a1-43f4-8573-a51f9cd9e870", "5c5bc609-bc7d-4be3-a030-43b4941a0902" });
        }
    }
}
