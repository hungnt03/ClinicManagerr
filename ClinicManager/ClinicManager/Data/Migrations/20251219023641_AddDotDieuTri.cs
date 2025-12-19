using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDotDieuTri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "macDinh",
                table: "GoiDieuTris");

            migrationBuilder.DropColumn(
                name: "gioBatDau",
                table: "BuoiDieuTris");

            migrationBuilder.DropColumn(
                name: "gioKetThuc",
                table: "BuoiDieuTris");

            migrationBuilder.RenameColumn(
                name: "tongSoBuoi",
                table: "GoiDieuTris",
                newName: "soBuoi");

            migrationBuilder.RenameColumn(
                name: "donGia",
                table: "GoiDieuTris",
                newName: "gia");

            migrationBuilder.RenameColumn(
                name: "trangThai",
                table: "BuoiDieuTris",
                newName: "noiDungTap");

            migrationBuilder.RenameColumn(
                name: "benhNhanGoiDieuTriId",
                table: "BuoiDieuTris",
                newName: "dotDieuTriId");

            migrationBuilder.AddColumn<int>(
                name: "bacSiDieuTriTayId",
                table: "BuoiDieuTris",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "chiPhiThuocVatTu",
                table: "BuoiDieuTris",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "kyThuatVienTapId",
                table: "BuoiDieuTris",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "noiDungDieuTriTay",
                table: "BuoiDieuTris",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "taoLuc",
                table: "BuoiDieuTris",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bacSiDieuTriTayId",
                table: "BuoiDieuTris");

            migrationBuilder.DropColumn(
                name: "chiPhiThuocVatTu",
                table: "BuoiDieuTris");

            migrationBuilder.DropColumn(
                name: "kyThuatVienTapId",
                table: "BuoiDieuTris");

            migrationBuilder.DropColumn(
                name: "noiDungDieuTriTay",
                table: "BuoiDieuTris");

            migrationBuilder.DropColumn(
                name: "taoLuc",
                table: "BuoiDieuTris");

            migrationBuilder.RenameColumn(
                name: "soBuoi",
                table: "GoiDieuTris",
                newName: "tongSoBuoi");

            migrationBuilder.RenameColumn(
                name: "gia",
                table: "GoiDieuTris",
                newName: "donGia");

            migrationBuilder.RenameColumn(
                name: "noiDungTap",
                table: "BuoiDieuTris",
                newName: "trangThai");

            migrationBuilder.RenameColumn(
                name: "dotDieuTriId",
                table: "BuoiDieuTris",
                newName: "benhNhanGoiDieuTriId");

            migrationBuilder.AddColumn<bool>(
                name: "macDinh",
                table: "GoiDieuTris",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "gioBatDau",
                table: "BuoiDieuTris",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "gioKetThuc",
                table: "BuoiDieuTris",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
