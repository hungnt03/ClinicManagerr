using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyCauHinhLuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "heSoTangCaNgayLe",
                table: "CauHinhLuongs");

            migrationBuilder.DropColumn(
                name: "nhanVienId",
                table: "BangLuongThangs");

            migrationBuilder.RenameColumn(
                name: "heSoTangCaNgayThuong",
                table: "CauHinhLuongs",
                newName: "donGiaTangCaMoiGio");

            migrationBuilder.AddColumn<int>(
                name: "nhanVienId",
                table: "BangLuongThangChiTiets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DotDieuTris_benhNhanId",
                table: "DotDieuTris",
                column: "benhNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_BuoiDieuTris_benhNhanId",
                table: "BuoiDieuTris",
                column: "benhNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_BangLuongThangChiTiets_nhanVienId",
                table: "BangLuongThangChiTiets",
                column: "nhanVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_BangLuongThangChiTiets_NhanViens_nhanVienId",
                table: "BangLuongThangChiTiets",
                column: "nhanVienId",
                principalTable: "NhanViens",
                principalColumn: "nhanVienId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
    name: "FK_BuoiDieuTris_BenhNhans_benhNhanId",
    table: "BuoiDieuTris",
    column: "benhNhanId",
    principalTable: "BenhNhans",
    principalColumn: "benhNhanId",
    onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotDieuTris_BenhNhans_benhNhanId",
                table: "DotDieuTris",
                column: "benhNhanId",
                principalTable: "BenhNhans",
                principalColumn: "benhNhanId",
                onDelete: ReferentialAction.Restrict);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BangLuongThangChiTiets_NhanViens_nhanVienId",
                table: "BangLuongThangChiTiets");

            migrationBuilder.DropForeignKey(
                name: "FK_BuoiDieuTris_BenhNhans_benhNhanId",
                table: "BuoiDieuTris");

            migrationBuilder.DropForeignKey(
                name: "FK_DotDieuTris_BenhNhans_benhNhanId",
                table: "DotDieuTris");

            migrationBuilder.DropIndex(
                name: "IX_DotDieuTris_benhNhanId",
                table: "DotDieuTris");

            migrationBuilder.DropIndex(
                name: "IX_BuoiDieuTris_benhNhanId",
                table: "BuoiDieuTris");

            migrationBuilder.DropIndex(
                name: "IX_BangLuongThangChiTiets_nhanVienId",
                table: "BangLuongThangChiTiets");

            migrationBuilder.DropColumn(
                name: "nhanVienId",
                table: "BangLuongThangChiTiets");

            migrationBuilder.RenameColumn(
                name: "donGiaTangCaMoiGio",
                table: "CauHinhLuongs",
                newName: "heSoTangCaNgayThuong");

            migrationBuilder.AddColumn<decimal>(
                name: "heSoTangCaNgayLe",
                table: "CauHinhLuongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "nhanVienId",
                table: "BangLuongThangs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
