using ClinicManager.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public interface IGoiDieuTriService
    {
        Task<List<SelectListItem>> GetGoiHoatDongAsync();
    }
    public class GoiDieuTriService : IGoiDieuTriService
    {
        private readonly ApplicationDbContext _context;

        public GoiDieuTriService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> GetGoiHoatDongAsync()
        {
            return await _context.GoiDieuTris
                .Where(x => x.hoatDong)
                .OrderBy(x => x.soBuoi)
                .Select(x => new SelectListItem
                {
                    Value = x.goiDieuTriId.ToString(),
                    Text = $"{x.tenGoi} - {x.soBuoi} buoi - {x.gia:N0} VND"
                })
                .ToListAsync();
        }
    }
}