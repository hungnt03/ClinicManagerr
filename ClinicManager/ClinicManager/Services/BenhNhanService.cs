using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.ViewModels.DotDieuTri;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public interface IBenhNhanService
    {
        Task<BenhNhan?> GetByIdAsync(int id);
        Task<List<BenhNhan>> GetAllAsync();

        Task<int> TaoBenhNhanAsync(
            string hoTen,
            DateTime? ngaySinh,
            string dienThoai,
            string diaChi,
            int? nhanVienGioiThieuId
        );

        Task<DotDieuTriChiTietVm?> GetChiTietDotDieuTriAsync(int dotDieuTriId);
    }
    public class BenhNhanService : IBenhNhanService
    {
        private readonly ApplicationDbContext _context;

        public BenhNhanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BenhNhan>> GetAllAsync()
        {
            return await _context.BenhNhans
                .OrderBy(x => x.hoTen)
                .ToListAsync();
        }

        public async Task<BenhNhan?> GetByIdAsync(int id)
        {
            return await _context.BenhNhans
                .FirstOrDefaultAsync(x => x.benhNhanId == id);
        }

        public async Task<int> TaoBenhNhanAsync(
            string hoTen,
            DateTime? ngaySinh,
            string dienThoai,
            string diaChi,
            int? nhanVienGioiThieuId)
        {
            var bn = new BenhNhan
            {
                hoTen = hoTen,
                ngaySinh = ngaySinh,
                soDienThoai = dienThoai,
                diaChi = diaChi,
                nguoiGioiThieuId = nhanVienGioiThieuId,
                taoLuc = DateTime.Now
            };

            _context.BenhNhans.Add(bn);
            await _context.SaveChangesAsync();

            return bn.benhNhanId;
        }

        // ===== dùng cho DotDieuTri/ChiTiet =====
        public async Task<DotDieuTriChiTietVm?> GetChiTietDotDieuTriAsync(int dotDieuTriId)
        {
            return await _context.DotDieuTris
                .Where(x => x.dotDieuTriId == dotDieuTriId)
                .Select(x => new DotDieuTriChiTietVm
                {
                    DotDieuTriId = x.dotDieuTriId,
                    BenhNhanId = x.benhNhanId,

                    NgayKham = x.ngayKham,
                    ChanDoan = x.chanDoan,
                    PhacDoDieuTri = x.phacDoDieuTri,

                    GoiDieuTriId = x.goiDieuTriId,
                    TenGoi = x.GoiDieuTri.tenGoi,

                    TongSoBuoi = x.tongSoBuoi,
                    SoBuoiDaDung = x.soBuoiDaDung,

                    TongTien = x.tongTien,
                    DaThanhToan = x.daThanhToan,

                    TrangThai = x.trangThai.ToString(),
                    TaoLuc = x.taoLuc
                })
                .FirstOrDefaultAsync();
        }
    }
}