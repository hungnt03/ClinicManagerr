using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDotDieuTri2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DotDieuTriMuaThems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dotDieuTriId = table.Column<int>(type: "int", nullable: false),
                    soBuoiThem = table.Column<int>(type: "int", nullable: false),
                    soTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    muaLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotDieuTriMuaThems", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "DotDieuTris",
                columns: table => new
                {
                    dotDieuTriId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    benhNhanId = table.Column<int>(type: "int", nullable: false),
                    bacSiKhamId = table.Column<int>(type: "int", nullable: false),
                    ngayKham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    chanDoan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phacDoDieuTri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    goiDieuTriId = table.Column<int>(type: "int", nullable: false),
                    tongSoBuoi = table.Column<int>(type: "int", nullable: false),
                    soBuoiDaDung = table.Column<int>(type: "int", nullable: false),
                    tongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    daThanhToan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    trangThai = table.Column<int>(type: "int", nullable: false),
                    taoLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotDieuTris", x => x.dotDieuTriId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DotDieuTriMuaThems");

            migrationBuilder.DropTable(
                name: "DotDieuTris");
        }
    }
}
