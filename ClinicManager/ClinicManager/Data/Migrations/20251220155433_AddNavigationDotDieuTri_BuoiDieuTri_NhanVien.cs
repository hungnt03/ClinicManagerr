using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationDotDieuTri_BuoiDieuTri_NhanVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BuoiDieuTris_bacSiDieuTriTayId",
                table: "BuoiDieuTris",
                column: "bacSiDieuTriTayId");

            migrationBuilder.CreateIndex(
                name: "IX_BuoiDieuTris_dotDieuTriId",
                table: "BuoiDieuTris",
                column: "dotDieuTriId");

            migrationBuilder.CreateIndex(
                name: "IX_BuoiDieuTris_kyThuatVienTapId",
                table: "BuoiDieuTris",
                column: "kyThuatVienTapId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuoiDieuTris_DotDieuTris_dotDieuTriId",
                table: "BuoiDieuTris",
                column: "dotDieuTriId",
                principalTable: "DotDieuTris",
                principalColumn: "dotDieuTriId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BuoiDieuTris_NhanViens_bacSiDieuTriTayId",
                table: "BuoiDieuTris",
                column: "bacSiDieuTriTayId",
                principalTable: "NhanViens",
                principalColumn: "nhanVienId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BuoiDieuTris_NhanViens_kyThuatVienTapId",
                table: "BuoiDieuTris",
                column: "kyThuatVienTapId",
                principalTable: "NhanViens",
                principalColumn: "nhanVienId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuoiDieuTris_DotDieuTris_dotDieuTriId",
                table: "BuoiDieuTris");

            migrationBuilder.DropForeignKey(
                name: "FK_BuoiDieuTris_NhanViens_bacSiDieuTriTayId",
                table: "BuoiDieuTris");

            migrationBuilder.DropForeignKey(
                name: "FK_BuoiDieuTris_NhanViens_kyThuatVienTapId",
                table: "BuoiDieuTris");

            migrationBuilder.DropIndex(
                name: "IX_BuoiDieuTris_bacSiDieuTriTayId",
                table: "BuoiDieuTris");

            migrationBuilder.DropIndex(
                name: "IX_BuoiDieuTris_dotDieuTriId",
                table: "BuoiDieuTris");

            migrationBuilder.DropIndex(
                name: "IX_BuoiDieuTris_kyThuatVienTapId",
                table: "BuoiDieuTris");
        }
    }
}
