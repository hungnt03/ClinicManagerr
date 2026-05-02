using ClinicManager.Data;
using ClinicManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services.Luong
{
    public interface ILuongService
    {
        Task<int> TinhLuongThangAsync(int thang, int nam);
        Task ChotLuongAsync(int bangLuongThangId);
        Task MoChotLuongAsync(int bangLuongThangId);
    }

    public class LuongService : ILuongService
    {
        private readonly ApplicationDbContext _context;

        public LuongService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================================================
        // TÍNH LƯƠNG THEO THÁNG
        // 1 THÁNG = 1 BangLuongThang
        // ==================================================
        public async Task<int> TinhLuongThangAsync(int thang, int nam)
        {
            // 1️⃣ Không cho tính lại
            if (await _context.BangLuongThangs
                .AnyAsync(x => x.thang == thang && x.nam == nam))
            {
                throw new Exception("Bảng lương tháng này đã tồn tại");
            }

            // 2️⃣ Cấu hình lương áp dụng
            var cauHinh = await _context.CauHinhLuongs
                .Where(x => x.apDungTuNgay <= new DateTime(nam, thang, 1))
                .OrderByDescending(x => x.apDungTuNgay)
                .FirstAsync();

            // 3️⃣ Ngày lễ
            var ngayLes = await _context.NgayLes
                .Where(x => x.ngay.Month == thang && x.ngay.Year == nam)
                .ToListAsync();

            // 4️⃣ Nhân viên đang hoạt động
            var nhanViens = await _context.NhanViens
                .Where(x => x.hoatDong)
                .ToListAsync();

            // 5️⃣ Tạo bảng lương tháng (DUY NHẤT)
            var bangLuong = new BangLuongThang
            {
                thang = thang,
                nam = nam,
                taoLuc = DateTime.Now,
                daChot = false
            };

            // 6️⃣ Tính số ngày làm chuẩn
            int soNgayTrongThang = DateTime.DaysInMonth(nam, thang);
            int soChuNhat = Enumerable.Range(1, soNgayTrongThang)
                .Count(d => new DateTime(nam, thang, d).DayOfWeek == DayOfWeek.Sunday);

            int soNgayLeTinhLuong = ngayLes.Count(x => x.coTinhLuong);

            int soNgayLamChuan =
                soNgayTrongThang - soChuNhat - soNgayLeTinhLuong;

            // 7️⃣ Duyệt từng nhân viên
            foreach (var nv in nhanViens)
            {
                // ===== CHẤM CÔNG =====
                var chamCongs = await _context.ChamCongs
                    .Where(x =>
                        x.nhanVienId == nv.nhanVienId &&
                        x.thoiGianVao.Month == thang &&
                        x.thoiGianVao.Year == nam)
                    .ToListAsync();

                int soNgayLam = chamCongs.Count(x => !x.nghiPhep);

                // ===== LƯƠNG CƠ BẢN =====
                decimal luongCoBan =
                    nv.luongCoBan * soNgayLam / soNgayLamChuan;

                bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                {
                    nhanVienId = nv.nhanVienId,
                    loai = LoaiLuongChiTiet.LuongCoBan,
                    soTien = Math.Round(luongCoBan, 0),
                    dienGiai = $"Lương cơ bản ({soNgayLam}/{soNgayLamChuan} ngày)"
                });

                // ===== ĂN TRƯA =====
                int soNgayAnTrua = chamCongs.Count(x => x.anTrua);
                if (soNgayAnTrua > 0)
                {
                    bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                    {
                        nhanVienId = nv.nhanVienId,
                        loai = LoaiLuongChiTiet.AnTrua,
                        soTien = soNgayAnTrua * 30000,
                        dienGiai = $"Ăn trưa ({soNgayAnTrua} ngày)"
                    });
                }

                // ===== XĂNG XE =====
                bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                {
                    nhanVienId = nv.nhanVienId,
                    loai = LoaiLuongChiTiet.XangXe,
                    soTien = 500000,
                    dienGiai = "Xăng xe"
                });

                // ===== CHUYÊN CẦN =====
                if (soNgayLam == soNgayLamChuan)
                {
                    bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                    {
                        nhanVienId = nv.nhanVienId,
                        loai = LoaiLuongChiTiet.ChuyenCan,
                        soTien = 200000,
                        dienGiai = "Chuyên cần"
                    });
                }

                // ===== TĂNG CA (OT – FIXED RATE) =====
                decimal tongOT = 0;

                foreach (var cc in chamCongs)
                {
                    tongOT += LuongTangCaHelper.TinhOTChoNgay(
                        cc,
                        cauHinh);
                }

                if (tongOT > 0)
                {
                    bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                    {
                        nhanVienId = nv.nhanVienId,
                        loai = LoaiLuongChiTiet.TangCa,
                        soTien = tongOT,
                        dienGiai = "Tăng ca (sau 16h)"
                    });
                }
            }
            // 8️⃣ Tổng lương tháng
            bangLuong.tongLuong = bangLuong.ChiTiets.Sum(x => x.soTien);
            _context.BangLuongThangs.Add(bangLuong);
            await _context.SaveChangesAsync();

            return bangLuong.bangLuongThangId;
        }

        public async Task ChotLuongAsync(int bangLuongThangId)
        {
            var bangLuong = await _context.BangLuongThangs
                .FirstOrDefaultAsync(x => x.bangLuongThangId == bangLuongThangId);

            if (bangLuong == null)
                throw new Exception("Không tìm thấy bảng lương");

            if (bangLuong.daChot)
                throw new Exception("Bảng lương đã được chốt");

            bangLuong.daChot = true;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Mở chốt (ADMIN ONLY)
        /// </summary>
        /// <param name="bangLuongThangId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task MoChotLuongAsync(int bangLuongThangId)
        {
            var bangLuong = await _context.BangLuongThangs
                .FirstOrDefaultAsync(x => x.bangLuongThangId == bangLuongThangId);

            if (bangLuong == null)
                throw new Exception("Không tim thấy bảng lương");

            bangLuong.daChot = false;

            await _context.SaveChangesAsync();
        }

    }
}