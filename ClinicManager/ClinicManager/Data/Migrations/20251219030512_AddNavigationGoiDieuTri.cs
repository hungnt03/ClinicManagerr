using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationGoiDieuTri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DotDieuTris_goiDieuTriId",
                table: "DotDieuTris",
                column: "goiDieuTriId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotDieuTris_GoiDieuTris_goiDieuTriId",
                table: "DotDieuTris",
                column: "goiDieuTriId",
                principalTable: "GoiDieuTris",
                principalColumn: "goiDieuTriId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotDieuTris_GoiDieuTris_goiDieuTriId",
                table: "DotDieuTris");

            migrationBuilder.DropIndex(
                name: "IX_DotDieuTris_goiDieuTriId",
                table: "DotDieuTris");
        }
    }
}
