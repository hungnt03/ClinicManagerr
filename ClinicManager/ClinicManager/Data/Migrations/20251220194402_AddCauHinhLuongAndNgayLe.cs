using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCauHinhLuongAndNgayLe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CauHinhLuongs",
                columns: table => new
                {
                    cauHinhLuongId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    gioBatDauSang = table.Column<TimeSpan>(type: "time", nullable: false),
                    gioKetThucSang = table.Column<TimeSpan>(type: "time", nullable: false),
                    gioBatDauChieu = table.Column<TimeSpan>(type: "time", nullable: false),
                    gioKetThucChieu = table.Column<TimeSpan>(type: "time", nullable: false),
                    soGioLamChuanNgay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tienAnTruaNgay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tienXangXeThang = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tienChuyenCan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tienDieuTriTayMoiBuoi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tienTapMoiBuoi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    phanTramGioiThieu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    heSoTangCaNgayThuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    heSoTangCaNgayLe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    soPhutLamTronTangCa = table.Column<int>(type: "int", nullable: false),
                    soPhutToiThieuTinhTangCa = table.Column<int>(type: "int", nullable: false),
                    apDungTuNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    taoLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhLuongs", x => x.cauHinhLuongId);
                });

            migrationBuilder.CreateTable(
                name: "NgayLes",
                columns: table => new
                {
                    ngayLeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ngay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    coTinhLuong = table.Column<bool>(type: "bit", nullable: false),
                    taoLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NgayLes", x => x.ngayLeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHinhLuongs");

            migrationBuilder.DropTable(
                name: "NgayLes");
        }
    }
}
