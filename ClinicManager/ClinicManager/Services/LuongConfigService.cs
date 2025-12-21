using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.ViewModels.Luong;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public interface ILuongConfigService
    {
        Task<List<CauHinhLuongVm>> GetAllAsync();
        Task<CauHinhLuongVm?> GetByIdAsync(int id);
        Task CreateAsync(CauHinhLuongVm vm);
    }
    public class LuongConfigService : ILuongConfigService
    {
        private readonly ApplicationDbContext _context;

        public LuongConfigService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CauHinhLuongVm>> GetAllAsync()
        {
            return await _context.CauHinhLuongs
                .OrderByDescending(x => x.apDungTuNgay)
                .Select(x => new CauHinhLuongVm
                {
                    CauHinhLuongId = x.cauHinhLuongId,
                    GioBatDauSang = x.gioBatDauSang,
                    GioKetThucSang = x.gioKetThucSang,
                    GioBatDauChieu = x.gioBatDauChieu,
                    GioKetThucChieu = x.gioKetThucChieu,
                    SoGioLamChuanNgay = x.soGioLamChuanNgay,

                    TienAnTruaNgay = x.tienAnTruaNgay,
                    TienXangXeThang = x.tienXangXeThang,
                    TienChuyenCan = x.tienChuyenCan,

                    TienDieuTriTayMoiBuoi = x.tienDieuTriTayMoiBuoi,
                    TienTapMoiBuoi = x.tienTapMoiBuoi,
                    PhanTramGioiThieu = x.phanTramGioiThieu,

                    HeSoTangCaNgayThuong = x.heSoTangCaNgayThuong,
                    HeSoTangCaNgayLe = x.heSoTangCaNgayLe,

                    SoPhutLamTronTangCa = x.soPhutLamTronTangCa,
                    SoPhutToiThieuTinhTangCa = x.soPhutToiThieuTinhTangCa,

                    ApDungTuNgay = x.apDungTuNgay
                })
                .ToListAsync();
        }

        public async Task<CauHinhLuongVm?> GetByIdAsync(int id)
        {
            var x = await _context.CauHinhLuongs.FindAsync(id);
            if (x == null) return null;

            return new CauHinhLuongVm
            {
                CauHinhLuongId = x.cauHinhLuongId,
                GioBatDauSang = x.gioBatDauSang,
                GioKetThucSang = x.gioKetThucSang,
                GioBatDauChieu = x.gioBatDauChieu,
                GioKetThucChieu = x.gioKetThucChieu,
                SoGioLamChuanNgay = x.soGioLamChuanNgay,

                TienAnTruaNgay = x.tienAnTruaNgay,
                TienXangXeThang = x.tienXangXeThang,
                TienChuyenCan = x.tienChuyenCan,

                TienDieuTriTayMoiBuoi = x.tienDieuTriTayMoiBuoi,
                TienTapMoiBuoi = x.tienTapMoiBuoi,
                PhanTramGioiThieu = x.phanTramGioiThieu,

                HeSoTangCaNgayThuong = x.heSoTangCaNgayThuong,
                HeSoTangCaNgayLe = x.heSoTangCaNgayLe,

                SoPhutLamTronTangCa = x.soPhutLamTronTangCa,
                SoPhutToiThieuTinhTangCa = x.soPhutToiThieuTinhTangCa,

                ApDungTuNgay = x.apDungTuNgay
            };
        }

        public async Task CreateAsync(CauHinhLuongVm vm)
        {
            var entity = new CauHinhLuong
            {
                gioBatDauSang = vm.GioBatDauSang,
                gioKetThucSang = vm.GioKetThucSang,
                gioBatDauChieu = vm.GioBatDauChieu,
                gioKetThucChieu = vm.GioKetThucChieu,
                soGioLamChuanNgay = vm.SoGioLamChuanNgay,

                tienAnTruaNgay = vm.TienAnTruaNgay,
                tienXangXeThang = vm.TienXangXeThang,
                tienChuyenCan = vm.TienChuyenCan,

                tienDieuTriTayMoiBuoi = vm.TienDieuTriTayMoiBuoi,
                tienTapMoiBuoi = vm.TienTapMoiBuoi,
                phanTramGioiThieu = vm.PhanTramGioiThieu,

                heSoTangCaNgayThuong = vm.HeSoTangCaNgayThuong,
                heSoTangCaNgayLe = vm.HeSoTangCaNgayLe,

                soPhutLamTronTangCa = vm.SoPhutLamTronTangCa,
                soPhutToiThieuTinhTangCa = vm.SoPhutToiThieuTinhTangCa,

                apDungTuNgay = vm.ApDungTuNgay,
                taoLuc = DateTime.Now
            };

            _context.CauHinhLuongs.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}