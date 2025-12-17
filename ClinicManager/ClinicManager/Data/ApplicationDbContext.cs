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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<KhamBenh>()
                .Property(x => x.goiDieuTriId)
                .IsRequired();
        }
    }
}
