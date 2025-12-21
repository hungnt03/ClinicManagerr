using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBuoiDieuTriAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "loai",
                table: "VatTus",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "BuoiDieuTriAudits",
                columns: table => new
                {
                    buoiDieuTriAuditId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    buoiDieuTriId = table.Column<int>(type: "int", nullable: false),
                    adminNhanVienId = table.Column<int>(type: "int", nullable: false),
                    ngayDieuTriCu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    bacSiDieuTriTayIdCu = table.Column<int>(type: "int", nullable: true),
                    kyThuatVienTapIdCu = table.Column<int>(type: "int", nullable: true),
                    noiDungTapCu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    noiDungDieuTriTayCu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    chiDinhDacBietCu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    chiPhiThuocVatTuCu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ngayDieuTriMoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    bacSiDieuTriTayIdMoi = table.Column<int>(type: "int", nullable: true),
                    kyThuatVienTapIdMoi = table.Column<int>(type: "int", nullable: true),
                    noiDungTapMoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    noiDungDieuTriTayMoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    chiDinhDacBietMoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    chiPhiThuocVatTuMoi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    lyDo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    suaLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuoiDieuTriAudits", x => x.buoiDieuTriAuditId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuoiDieuTriAudits");

            migrationBuilder.AlterColumn<string>(
                name: "loai",
                table: "VatTus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
