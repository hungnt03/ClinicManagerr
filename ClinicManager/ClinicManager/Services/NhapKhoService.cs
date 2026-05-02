using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.ViewModels.NhapKho;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public interface INhapKhoService
    {
        Task<int> TaoPhieuNhapAsync(
            NhapKhoCreateVm vm,
            int nhanVienNhapId);
    }
    public class NhapKhoService : INhapKhoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public NhapKhoService(
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<int> TaoPhieuNhapAsync(
            NhapKhoCreateVm vm,
            int nhanVienNhapId)
        {
            if (vm.Items.Count == 0)
                throw new Exception("Phiếu nhập phải có ít nhất 1 vật tư");

            string? duongDanHoaDon = null;

            // ===== LUU FILE HOA DON =====
            if (vm.HoaDonFile != null && vm.HoaDonFile.Length > 0)
            {
                var folder = Path.Combine(
                    _env.WebRootPath,
                    "Bills",
                    "NhapKho");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName =
                    $"PN_{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(vm.HoaDonFile.FileName)}";

                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await vm.HoaDonFile.CopyToAsync(stream);
                }

                duongDanHoaDon = $"Bills/NhapKho/{fileName}";
            }

            var phieu = new PhieuNhapKho
            {
                ngayNhap = vm.NgayNhap,
                ghiChu = vm.GhiChu,
                nhanVienNhapId = nhanVienNhapId,
                duongDanHoaDon = duongDanHoaDon,
                taoLuc = DateTime.Now
            };

            _context.PhieuNhapKhos.Add(phieu);
            await _context.SaveChangesAsync();

            decimal tongTien = 0;

            foreach (var item in vm.Items)
            {
                var vatTu = await _context.VatTus
                    .FirstOrDefaultAsync(x => x.vatTuId == item.VatTuId);

                if (vatTu == null)
                    throw new Exception("Vật tư không tồn tại");

                vatTu.tonKho += item.SoLuong;

                var ct = new PhieuNhapKhoChiTiet
                {
                    phieuNhapKhoId = phieu.phieuNhapKhoId,
                    vatTuId = item.VatTuId,
                    soLuong = item.SoLuong,
                    donGiaNhap = item.DonGiaNhap,
                    thanhTien = item.SoLuong * item.DonGiaNhap
                };

                tongTien += ct.thanhTien;
                _context.PhieuNhapKhoChiTiets.Add(ct);
            }

            phieu.tongTien = tongTien;

            await _context.SaveChangesAsync();

            return phieu.phieuNhapKhoId;
        }
    }
}