using ClinicManager.Data;
using ClinicManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class ChamCongAdminService : IChamCongAdminService
    {
        private readonly ApplicationDbContext _context;

        public ChamCongAdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SuaGioVaoRaAsync(
            int chamCongId,
            DateTime? gioVaoMoi,
            DateTime? gioRaMoi,
            int adminNhanVienId,
            string lyDo)
        {
            if (string.IsNullOrWhiteSpace(lyDo))
                throw new Exception("Bat buoc nhap ly do");

            var chamCong = await _context.ChamCongs
                .FirstOrDefaultAsync(x => x.chamCongId == chamCongId);

            if (chamCong == null)
                throw new Exception("Cham cong khong ton tai");

            // Audit
            var audit = new ChamCongAudit
            {
                chamCongId = chamCongId,
                adminNhanVienId = adminNhanVienId,

                thoiGianVaoCu = chamCong.thoiGianVao,
                thoiGianRaCu = chamCong.thoiGianRa,
                nghiPhepCu = chamCong.nghiPhep,
                nghiPhepCoLuongCu = chamCong.nghiPhepCoLuong,

                thoiGianVaoMoi = gioVaoMoi,
                thoiGianRaMoi = gioRaMoi,
                nghiPhepMoi = chamCong.nghiPhep,
                nghiPhepCoLuongMoi = chamCong.nghiPhepCoLuong,

                lyDo = lyDo,
                suaLuc = DateTime.Now
            };

            chamCong.thoiGianVao = gioVaoMoi ?? chamCong.thoiGianVao;
            chamCong.thoiGianRa = gioRaMoi;

            _context.ChamCongAudits.Add(audit);
            await _context.SaveChangesAsync();
        }

        public async Task SuaNghiPhepAsync(
            int chamCongId,
            bool nghiPhep,
            bool nghiPhepCoLuong,
            int adminNhanVienId,
            string lyDo)
        {
            if (string.IsNullOrWhiteSpace(lyDo))
                throw new Exception("Bat buoc nhap ly do");

            var chamCong = await _context.ChamCongs
                .FirstOrDefaultAsync(x => x.chamCongId == chamCongId);

            if (chamCong == null)
                throw new Exception("Cham cong khong ton tai");

            var audit = new ChamCongAudit
            {
                chamCongId = chamCongId,
                adminNhanVienId = adminNhanVienId,

                thoiGianVaoCu = chamCong.thoiGianVao,
                thoiGianRaCu = chamCong.thoiGianRa,
                nghiPhepCu = chamCong.nghiPhep,
                nghiPhepCoLuongCu = chamCong.nghiPhepCoLuong,

                thoiGianVaoMoi = chamCong.thoiGianVao,
                thoiGianRaMoi = chamCong.thoiGianRa,
                nghiPhepMoi = nghiPhep,
                nghiPhepCoLuongMoi = nghiPhepCoLuong,

                lyDo = lyDo,
                suaLuc = DateTime.Now
            };

            chamCong.nghiPhep = nghiPhep;
            chamCong.nghiPhepCoLuong = nghiPhepCoLuong;

            _context.ChamCongAudits.Add(audit);
            await _context.SaveChangesAsync();
        }
    }

    public interface IChamCongAdminService
    {
        Task SuaGioVaoRaAsync(
            int chamCongId,
            DateTime? gioVaoMoi,
            DateTime? gioRaMoi,
            int adminNhanVienId,
            string lyDo
        );

        Task SuaNghiPhepAsync(
            int chamCongId,
            bool nghiPhep,
            bool nghiPhepCoLuong,
            int adminNhanVienId,
            string lyDo
        );
    }
}
