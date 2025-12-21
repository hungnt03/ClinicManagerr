using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBangLuongThang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BangLuongThangs",
                columns: table => new
                {
                    bangLuongThangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nhanVienId = table.Column<int>(type: "int", nullable: false),
                    thang = table.Column<int>(type: "int", nullable: false),
                    nam = table.Column<int>(type: "int", nullable: false),
                    tongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    daChot = table.Column<bool>(type: "bit", nullable: false),
                    taoLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangLuongThangs", x => x.bangLuongThangId);
                });

            migrationBuilder.CreateTable(
                name: "BangLuongThangChiTiets",
                columns: table => new
                {
                    bangLuongThangChiTietId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bangLuongThangId = table.Column<int>(type: "int", nullable: false),
                    loai = table.Column<int>(type: "int", nullable: false),
                    soTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    dienGiai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangLuongThangChiTiets", x => x.bangLuongThangChiTietId);
                    table.ForeignKey(
                        name: "FK_BangLuongThangChiTiets_BangLuongThangs_bangLuongThangId",
                        column: x => x.bangLuongThangId,
                        principalTable: "BangLuongThangs",
                        principalColumn: "bangLuongThangId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangLuongThangChiTiets_bangLuongThangId",
                table: "BangLuongThangChiTiets",
                column: "bangLuongThangId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangLuongThangChiTiets");

            migrationBuilder.DropTable(
                name: "BangLuongThangs");
        }
    }
}
