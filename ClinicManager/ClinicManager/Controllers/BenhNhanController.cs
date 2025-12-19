using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.ViewModels.BenhNhan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Controllers
{
    [Authorize]
    public class BenhNhanController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BenhNhanController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: /BenhNhan
        public async Task<IActionResult> Index()
        {
            var benhNhans = await _context.BenhNhans
                .OrderByDescending(x => x.taoLuc)
                .ToListAsync();

            var nhanViens = await _context.NhanViens.ToListAsync();

            var result = benhNhans.Select(bn =>
            {
                var nguoiGt = nhanViens
                    .FirstOrDefault(x => x.nhanVienId == bn.nguoiGioiThieuId);

                int? tuoi = null;
                if (bn.ngaySinh.HasValue)
                {
                    tuoi = DateTime.Now.Year - bn.ngaySinh.Value.Year;
                }

                return new BenhNhanListItemVm
                {
                    BenhNhanId = bn.benhNhanId,
                    HoTen = bn.hoTen,
                    GioiTinh = bn.gioiTinh,
                    Tuoi = tuoi,
                    SoDienThoai = bn.soDienThoai,
                    NguoiGioiThieu = nguoiGt?.hoTen
                };
            }).ToList();

            return View(result);
        }

        // GET: /BenhNhan/Tao
        public async Task<IActionResult> Tao()
        {
            var vm = new BenhNhanFormVm
            {
                DanhSachNhanVien = await _context.NhanViens
                    .Where(x => x.hoatDong)
                    .Select(x => new NhanVienOptionVm
                    {
                        NhanVienId = x.nhanVienId,
                        HoTen = x.hoTen
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        // POST: /BenhNhan/Tao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tao(BenhNhanFormVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.DanhSachNhanVien = await _context.NhanViens
                    .Where(x => x.hoatDong)
                    .Select(x => new NhanVienOptionVm
                    {
                        NhanVienId = x.nhanVienId,
                        HoTen = x.hoTen
                    })
                    .ToListAsync();

                return View(vm);
            }

            var benhNhan = new BenhNhan
            {
                hoTen = vm.HoTen,
                ngaySinh = vm.NgaySinh,
                gioiTinh = vm.GioiTinh,
                soDienThoai = vm.SoDienThoai,
                diaChi = vm.DiaChi,
                nguoiGioiThieuId = vm.NguoiGioiThieuId,
                taoLuc = DateTime.Now
            };

            _context.BenhNhans.Add(benhNhan);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: /BenhNhan/Sua/5
        public async Task<IActionResult> Sua(int id)
        {
            var bn = await _context.BenhNhans
                .FirstOrDefaultAsync(x => x.benhNhanId == id);

            if (bn == null)
                return NotFound();

            var vm = new BenhNhanFormVm
            {
                BenhNhanId = bn.benhNhanId,
                HoTen = bn.hoTen,
                NgaySinh = bn.ngaySinh,
                GioiTinh = bn.gioiTinh,
                SoDienThoai = bn.soDienThoai,
                DiaChi = bn.diaChi,
                NguoiGioiThieuId = bn.nguoiGioiThieuId,

                DanhSachNhanVien = await _context.NhanViens
                    .Where(x => x.hoatDong)
                    .Select(x => new NhanVienOptionVm
                    {
                        NhanVienId = x.nhanVienId,
                        HoTen = x.hoTen
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        // POST: /BenhNhan/Sua
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(BenhNhanFormVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.DanhSachNhanVien = await _context.NhanViens
                    .Where(x => x.hoatDong)
                    .Select(x => new NhanVienOptionVm
                    {
                        NhanVienId = x.nhanVienId,
                        HoTen = x.hoTen
                    })
                    .ToListAsync();

                return View(vm);
            }

            var bn = await _context.BenhNhans
                .FirstOrDefaultAsync(x => x.benhNhanId == vm.BenhNhanId);

            if (bn == null)
                return NotFound();

            bn.hoTen = vm.HoTen;
            bn.ngaySinh = vm.NgaySinh;
            bn.gioiTinh = vm.GioiTinh;
            bn.soDienThoai = vm.SoDienThoai;
            bn.diaChi = vm.DiaChi;
            bn.nguoiGioiThieuId = vm.NguoiGioiThieuId;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: /BenhNhan/ChiTiet/5
        public async Task<IActionResult> ChiTiet(int id)
        {
            var bn = await _context.BenhNhans
                .FirstOrDefaultAsync(x => x.benhNhanId == id);

            if (bn == null)
                return NotFound();

            var dotDieuTris = await _context.DotDieuTris
                .Where(x => x.benhNhanId == id)
                .OrderByDescending(x => x.ngayKham)
                .Select(x => new DotDieuTriItemVm
                {
                    DotDieuTriId = x.dotDieuTriId,
                    NgayKham = x.ngayKham,
                    ChanDoan = x.chanDoan,
                    PhacDoDieuTri = x.phacDoDieuTri,
                    TongSoBuoi = x.tongSoBuoi,
                    SoBuoiDaDung = x.soBuoiDaDung,
                    TongTien = x.tongTien,
                    DaThanhToan = x.daThanhToan,
                    TrangThai = x.trangThai.ToString()
                })
                .ToListAsync();

            var vm = new BenhNhanChiTietVm
            {
                BenhNhanId = bn.benhNhanId,
                HoTen = bn.hoTen,
                NgaySinh = bn.ngaySinh,
                GioiTinh = bn.gioiTinh,
                SoDienThoai = bn.soDienThoai,
                DiaChi = bn.diaChi,
                DotDieuTris = dotDieuTris
            };

            return View(vm);
        }

    }
}
