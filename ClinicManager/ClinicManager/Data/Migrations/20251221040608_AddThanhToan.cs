using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddThanhToan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ngayThanhToan",
                table: "DotDieuTris",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "phanTramGiamGia",
                table: "DotDieuTris",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ThanhToans",
                columns: table => new
                {
                    thanhToanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    loai = table.Column<int>(type: "int", nullable: false),
                    dotDieuTriId = table.Column<int>(type: "int", nullable: true),
                    buoiDieuTriId = table.Column<int>(type: "int", nullable: true),
                    soTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ngayThu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    hinhThuc = table.Column<int>(type: "int", nullable: false),
                    ghiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    daChot = table.Column<bool>(type: "bit", nullable: false),
                    taoLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToans", x => x.thanhToanId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThanhToans");

            migrationBuilder.DropColumn(
                name: "ngayThanhToan",
                table: "DotDieuTris");

            migrationBuilder.DropColumn(
                name: "phanTramGiamGia",
                table: "DotDieuTris");
        }
    }
}
