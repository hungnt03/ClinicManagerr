using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitBussinessSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BenhNhanGoiDieuTris",
                columns: table => new
                {
                    benhNhanGoiDieuTriId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    benhNhanId = table.Column<int>(type: "int", nullable: false),
                    khamBenhId = table.Column<int>(type: "int", nullable: false),
                    goiDieuTriId = table.Column<int>(type: "int", nullable: false),
                    tongSoBuoi = table.Column<int>(type: "int", nullable: false),
                    soBuoiDaDung = table.Column<int>(type: "int", nullable: false),
                    ngayMua = table.Column<DateTime>(type: "datetime2", nullable: false),
                    trangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNhanGoiDieuTris", x => x.benhNhanGoiDieuTriId);
                });

            migrationBuilder.CreateTable(
                name: "BenhNhans",
                columns: table => new
                {
                    benhNhanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ngaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    soDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    diaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nguoiGioiThieuId = table.Column<int>(type: "int", nullable: true),
                    taoLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNhans", x => x.benhNhanId);
                });

            migrationBuilder.CreateTable(
                name: "BuoiDieuTriNhanViens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    buoiDieuTriId = table.Column<int>(type: "int", nullable: false),
                    nhanVienId = table.Column<int>(type: "int", nullable: false),
                    vaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuoiDieuTriNhanViens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "BuoiDieuTris",
                columns: table => new
                {
                    buoiDieuTriId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    benhNhanId = table.Column<int>(type: "int", nullable: false),
                    benhNhanGoiDieuTriId = table.Column<int>(type: "int", nullable: false),
                    ngayDieuTri = table.Column<DateTime>(type: "datetime2", nullable: false),
                    gioBatDau = table.Column<TimeSpan>(type: "time", nullable: false),
                    gioKetThuc = table.Column<TimeSpan>(type: "time", nullable: false),
                    chiDinhDacBiet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    trangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuoiDieuTris", x => x.buoiDieuTriId);
                });

            migrationBuilder.CreateTable(
                name: "ChamCongs",
                columns: table => new
                {
                    chamCongId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nhanVienId = table.Column<int>(type: "int", nullable: false),
                    thoiGianVao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    thoiGianRa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    nghiPhep = table.Column<bool>(type: "bit", nullable: false),
                    nghiPhepCoLuong = table.Column<bool>(type: "bit", nullable: false),
                    anTrua = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChamCongs", x => x.chamCongId);
                });

            migrationBuilder.CreateTable(
                name: "DatLichKhams",
                columns: table => new
                {
                    datLichKhamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    benhNhanId = table.Column<int>(type: "int", nullable: true),
                    hoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    soDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    thoiGianHen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    bacSiDuKienId = table.Column<int>(type: "int", nullable: true),
                    trangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ghiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    taoLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatLichKhams", x => x.datLichKhamId);
                });

            migrationBuilder.CreateTable(
                name: "GoiDieuTris",
                columns: table => new
                {
                    goiDieuTriId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenGoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tongSoBuoi = table.Column<int>(type: "int", nullable: false),
                    donGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    macDinh = table.Column<bool>(type: "bit", nullable: false),
                    hoatDong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoiDieuTris", x => x.goiDieuTriId);
                });

            migrationBuilder.CreateTable(
                name: "KhamBenhs",
                columns: table => new
                {
                    khamBenhId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    benhNhanId = table.Column<int>(type: "int", nullable: false),
                    bacSiId = table.Column<int>(type: "int", nullable: false),
                    ngayKham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    chanDoan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phacDoDieuTri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    goiDieuTriId = table.Column<int>(type: "int", nullable: false),
                    tinhTrangThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    taoLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhamBenhs", x => x.khamBenhId);
                });

            migrationBuilder.CreateTable(
                name: "NhanViens",
                columns: table => new
                {
                    nhanVienId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    luongCoBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    hoatDong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.nhanVienId);
                });

            migrationBuilder.CreateTable(
                name: "ThuocVatTuBuoiDieuTris",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    buoiDieuTriId = table.Column<int>(type: "int", nullable: false),
                    vatTuId = table.Column<int>(type: "int", nullable: false),
                    soLuong = table.Column<int>(type: "int", nullable: false),
                    donGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuocVatTuBuoiDieuTris", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "VatTus",
                columns: table => new
                {
                    vatTuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenVatTu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    loai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    donViTinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    donGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tonKho = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatTus", x => x.vatTuId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BenhNhanGoiDieuTris");

            migrationBuilder.DropTable(
                name: "BenhNhans");

            migrationBuilder.DropTable(
                name: "BuoiDieuTriNhanViens");

            migrationBuilder.DropTable(
                name: "BuoiDieuTris");

            migrationBuilder.DropTable(
                name: "ChamCongs");

            migrationBuilder.DropTable(
                name: "DatLichKhams");

            migrationBuilder.DropTable(
                name: "GoiDieuTris");

            migrationBuilder.DropTable(
                name: "KhamBenhs");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropTable(
                name: "ThuocVatTuBuoiDieuTris");

            migrationBuilder.DropTable(
                name: "VatTus");
        }
    }
}
