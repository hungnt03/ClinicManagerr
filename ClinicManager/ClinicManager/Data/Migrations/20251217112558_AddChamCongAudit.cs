using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChamCongAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChamCongAudits",
                columns: table => new
                {
                    chamCongAuditId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chamCongId = table.Column<int>(type: "int", nullable: false),
                    adminNhanVienId = table.Column<int>(type: "int", nullable: false),
                    thoiGianVaoCu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    thoiGianRaCu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    nghiPhepCu = table.Column<bool>(type: "bit", nullable: false),
                    nghiPhepCoLuongCu = table.Column<bool>(type: "bit", nullable: false),
                    thoiGianVaoMoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    thoiGianRaMoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    nghiPhepMoi = table.Column<bool>(type: "bit", nullable: false),
                    nghiPhepCoLuongMoi = table.Column<bool>(type: "bit", nullable: false),
                    lyDo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    suaLuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChamCongAudits", x => x.chamCongAuditId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChamCongAudits");
        }
    }
}
