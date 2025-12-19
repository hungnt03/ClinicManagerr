using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceToBuoiTap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ThuocVatTuBuoiDieuTris_buoiDieuTriId",
                table: "ThuocVatTuBuoiDieuTris",
                column: "buoiDieuTriId");

            migrationBuilder.CreateIndex(
                name: "IX_ThuocVatTuBuoiDieuTris_vatTuId",
                table: "ThuocVatTuBuoiDieuTris",
                column: "vatTuId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThuocVatTuBuoiDieuTris_BuoiDieuTris_buoiDieuTriId",
                table: "ThuocVatTuBuoiDieuTris",
                column: "buoiDieuTriId",
                principalTable: "BuoiDieuTris",
                principalColumn: "buoiDieuTriId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThuocVatTuBuoiDieuTris_VatTus_vatTuId",
                table: "ThuocVatTuBuoiDieuTris",
                column: "vatTuId",
                principalTable: "VatTus",
                principalColumn: "vatTuId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThuocVatTuBuoiDieuTris_BuoiDieuTris_buoiDieuTriId",
                table: "ThuocVatTuBuoiDieuTris");

            migrationBuilder.DropForeignKey(
                name: "FK_ThuocVatTuBuoiDieuTris_VatTus_vatTuId",
                table: "ThuocVatTuBuoiDieuTris");

            migrationBuilder.DropIndex(
                name: "IX_ThuocVatTuBuoiDieuTris_buoiDieuTriId",
                table: "ThuocVatTuBuoiDieuTris");

            migrationBuilder.DropIndex(
                name: "IX_ThuocVatTuBuoiDieuTris_vatTuId",
                table: "ThuocVatTuBuoiDieuTris");
        }
    }
}
