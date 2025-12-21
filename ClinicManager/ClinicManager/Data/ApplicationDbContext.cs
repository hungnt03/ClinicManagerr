using System.Reflection.Emit;
using ClinicManager.Models;
using ClinicManager.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BenhNhan> BenhNhans { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<GoiDieuTri> GoiDieuTris { get; set; }
        public DbSet<DatLichKham> DatLichKhams { get; set; }
        public DbSet<KhamBenh> KhamBenhs { get; set; }
        public DbSet<BenhNhanGoiDieuTri> BenhNhanGoiDieuTris { get; set; }
        public DbSet<BuoiDieuTri> BuoiDieuTris { get; set; }
        public DbSet<BuoiDieuTriNhanVien> BuoiDieuTriNhanViens { get; set; }
        public DbSet<VatTu> VatTus { get; set; }
        public DbSet<ThuocVatTuBuoiDieuTri> ThuocVatTuBuoiDieuTris { get; set; }
        public DbSet<ChamCong> ChamCongs { get; set; }
        public DbSet<ChamCongAudit> ChamCongAudits { get; set; }
        public DbSet<DotDieuTri> DotDieuTris { get; set; }
        public DbSet<DotDieuTriMuaThem> DotDieuTriMuaThems { get; set; }
        public DbSet<BuoiDieuTriAudit> BuoiDieuTriAudits { get; set; }
        public DbSet<PhieuNhapKho> PhieuNhapKhos { get; set; }
        public DbSet<PhieuNhapKhoChiTiet> PhieuNhapKhoChiTiets { get; set; }
        public DbSet<CauHinhLuong> CauHinhLuongs { get; set; }
        public DbSet<NgayLe> NgayLes { get; set; }
        public DbSet<BangLuongThang> BangLuongThangs { get; set; }
        public DbSet<BangLuongThangChiTiet> BangLuongThangChiTiets { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }





        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<KhamBenh>()
                .Property(x => x.goiDieuTriId)
                .IsRequired();

            builder.Entity<BuoiDieuTri>()
                .HasOne(b => b.BacSiDieuTriTay)
                .WithMany()
                .HasForeignKey(b => b.bacSiDieuTriTayId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BuoiDieuTri>()
                .HasOne(b => b.NguoiTap)
                .WithMany()
                .HasForeignKey(b => b.kyThuatVienTapId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BuoiDieuTri>()
                .HasOne(b => b.DotDieuTri)
                .WithMany(d => d.BuoiDieuTris)
                .HasForeignKey(b => b.dotDieuTriId);

            builder.Entity<BuoiDieuTri>()
                .HasOne(b => b.DotDieuTri)
                .WithMany(d => d.BuoiDieuTris)
                .HasForeignKey(b => b.dotDieuTriId);

        }
    }
}
