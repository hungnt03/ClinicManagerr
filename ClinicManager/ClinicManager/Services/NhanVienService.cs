using ClinicManager.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public interface INhanVienService
    {
        Task<List<SelectListItem>> GetDanhSachBacSiKyThuatViensync();
    }
    public class NhanVienService : INhanVienService
    {
        private readonly ApplicationDbContext _context;

        public NhanVienService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> GetDanhSachBacSiKyThuatViensync()
        {
            return await _context.NhanViens
                .Where(x => (x.vaiTro == "BacSi" || x.vaiTro == "KyThuatVien") && x.hoatDong)
                .Select(x => new SelectListItem
                {
                    Value = x.nhanVienId.ToString(),
                    Text = x.hoTen
                })
                .ToListAsync();
        }
    }
}
