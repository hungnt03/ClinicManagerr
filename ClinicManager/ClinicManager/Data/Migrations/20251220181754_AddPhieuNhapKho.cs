using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPhieuNhapKho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhieuNhapKhoChiTiets",
                columns: table => new
                {
                    phieuNhapKhoChiTietId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    phieuNhapKhoId = table.Column<int>(type: "int", nullable: false),
                    vatTuId = table.Column<int>(type: "int", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    donGiaNhap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    thanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNhapKhoChiTiets", x => x.phieuNhapKhoChiTietId);
                    table.ForeignKey(
                        name: "FK_PhieuNhapKhoChiTiets_VatTus_vatTuId",
                        column: x => x.vatTuId,
                        principalTable: "VatTus",
                        principalColumn: "vatTuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuNhapKhos",
                columns: table => new
                {
                    phieuNhapKhoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ngayNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ghiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nhanVienNhapId = table.Column<int>(type: "int", nullable: false),
                    tongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    taoLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNhapKhos", x => x.phieuNhapKhoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhapKhoChiTiets_vatTuId",
                table: "PhieuNhapKhoChiTiets",
                column: "vatTuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhieuNhapKhoChiTiets");

            migrationBuilder.DropTable(
                name: "PhieuNhapKhos");
        }
    }
}
