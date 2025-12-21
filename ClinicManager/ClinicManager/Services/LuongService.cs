using ClinicManager.Data;
using ClinicManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public interface ILuongService
    {
        Task<int> TinhLuongThangAsync(int thang, int nam);
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
                throw new Exception("Bang luong thang nay da ton tai");
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
                    dienGiai = $"Luong co ban ({soNgayLam}/{soNgayLamChuan} ngay)"
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
                        dienGiai = $"An trua ({soNgayAnTrua} ngay)"
                    });
                }

                // ===== XĂNG XE =====
                bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                {
                    nhanVienId = nv.nhanVienId,
                    loai = LoaiLuongChiTiet.XangXe,
                    soTien = 500000,
                    dienGiai = "Xang xe"
                });

                // ===== CHUYÊN CẦN =====
                if (soNgayLam == soNgayLamChuan)
                {
                    bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                    {
                        nhanVienId = nv.nhanVienId,
                        loai = LoaiLuongChiTiet.ChuyenCan,
                        soTien = 200000,
                        dienGiai = "Chuyen can"
                    });
                }

                // ===== TĂNG CA (OT) =====
                decimal luongMoiGio =
                    nv.luongCoBan / (soNgayLamChuan * cauHinh.soGioLamChuanNgay);

                decimal tongOT = 0;

                foreach (var cc in chamCongs)
                {
                    bool laNgayDacBiet =
                        LuongTangCaHelper.LaNgayLeHoacChuNhat(
                            cc.thoiGianVao.Date,
                            ngayLes);

                    tongOT += LuongTangCaHelper.TinhOTChoNgay(
                        cc,
                        cauHinh,
                        luongMoiGio,
                        laNgayDacBiet);
                }

                if (tongOT > 0)
                {
                    bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                    {
                        nhanVienId = nv.nhanVienId,
                        loai = LoaiLuongChiTiet.TangCa,
                        soTien = Math.Round(tongOT, 0),
                        dienGiai = "Tang ca"
                    });
                }
            }

            // 8️⃣ Tổng lương tháng
            bangLuong.tongLuong = bangLuong.ChiTiets.Sum(x => x.soTien);

            _context.BangLuongThangs.Add(bangLuong);
            await _context.SaveChangesAsync();

            return bangLuong.bangLuongThangId;
        }
        //public async Task TinhLuongThangAsync(int thang, int nam)
        //{
        //    // 1. LẤY CẤU HÌNH LƯƠNG HIỆU LỰC
        //    var cauHinh = await _context.CauHinhLuongs
        //        .Where(x => x.apDungTuNgay <= new DateTime(nam, thang, 1))
        //        .OrderByDescending(x => x.apDungTuNgay)
        //        .FirstAsync();

        //    // 2. DANH SÁCH NGÀY LỄ
        //    var ngayLes = await _context.NgayLes
        //        .Where(x => x.ngay.Month == thang && x.ngay.Year == nam)
        //        .ToListAsync();

        //    // 3. DANH SÁCH NHÂN VIÊN
        //    var nhanViens = await _context.NhanViens.ToListAsync();

        //    foreach (var nv in nhanViens)
        //    {
        //        // XÓA LƯƠNG CŨ NẾU CHƯA CHỐT
        //        var old = await _context.BangLuongThangs
        //            .Include(x => x.ChiTiets)
        //            .FirstOrDefaultAsync(x =>
        //                x.nhanVienId == nv.nhanVienId &&
        //                x.thang == thang &&
        //                x.nam == nam);

        //        if (old != null)
        //        {
        //            if (old.daChot)
        //                continue;

        //            _context.BangLuongThangChiTiets.RemoveRange(old.ChiTiets);
        //            _context.BangLuongThangs.Remove(old);
        //            await _context.SaveChangesAsync();
        //        }

        //        var bangLuong = new BangLuongThang
        //        {
        //            nhanVienId = nv.nhanVienId,
        //            thang = thang,
        //            nam = nam,
        //            taoLuc = DateTime.Now
        //        };

        //        // ================= NGÀY CÔNG =================
        //        var chamCongs = await _context.ChamCongs
        //            .Where(x =>
        //                x.nhanVienId == nv.nhanVienId &&
        //                x.thoiGianVao.Month == thang &&
        //                x.thoiGianVao.Year == nam)
        //            .ToListAsync();

        //        int soNgayDiLam = chamCongs
        //            .Select(x => x.thoiGianVao.Date)
        //            .Distinct()
        //            .Count();

        //        // ngày chuẩn trong tháng
        //        int soNgayTrongThang = DateTime.DaysInMonth(nam, thang);
        //        int soChuNhat = Enumerable.Range(1, soNgayTrongThang)
        //            .Select(d => new DateTime(nam, thang, d))
        //            .Count(d => d.DayOfWeek == DayOfWeek.Sunday);

        //        int soNgayLeKhongTinhLuong = ngayLes.Count(x => !x.coTinhLuong);                

        //        //TODO
        //        int soNgayLamChuan =
        //            soNgayTrongThang - soChuNhat - soNgayLeKhongTinhLuong;

        //        int soNgayLeTinhLuong = ngayLes.Count(x => x.coTinhLuong);

        //        int tongNgayHuongLuong =
        //            soNgayDiLam + soNgayLeTinhLuong;

        //        // LƯƠNG CƠ BẢN
        //        decimal luongCB =
        //            nv.luongCoBan * tongNgayHuongLuong / soNgayLamChuan;

        //        bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
        //        {
        //            loai = LoaiLuongChiTiet.LuongCoBan,
        //            soTien = luongCB,
        //            dienGiai = $"Luong co ban ({soNgayDiLam}/{soNgayLamChuan} ngay)"
        //        });

        //        // ================= ĂN TRƯA =================
        //        int soNgayAnTrua = chamCongs.Count(x => x.anTrua);
        //        decimal tienAnTrua =
        //            soNgayAnTrua * cauHinh.tienAnTruaNgay;

        //        if (tienAnTrua > 0)
        //        {
        //            bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
        //            {
        //                loai = LoaiLuongChiTiet.AnTrua,
        //                soTien = tienAnTrua,
        //                dienGiai = $"{soNgayAnTrua} ngay an trua"
        //            });
        //        }

        //        // ================= XĂNG XE =================
        //        bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
        //        {
        //            loai = LoaiLuongChiTiet.XangXe,
        //            soTien = cauHinh.tienXangXeThang,
        //            dienGiai = "Xang xe co dinh"
        //        });

        //        // ================= CHUYÊN CẦN =================
        //        bool coNghi = chamCongs.Any(x => x.nghiPhep);
        //        if (!coNghi)
        //        {
        //            bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
        //            {
        //                loai = LoaiLuongChiTiet.ChuyenCan,
        //                soTien = cauHinh.tienChuyenCan,
        //                dienGiai = "Chuyen can"
        //            });
        //        }

        //        // ================= HOA HỒNG BUỔI =================
        //        var buois = await _context.BuoiDieuTris
        //            .Where(x =>
        //                x.ngayDieuTri.Month == thang &&
        //                x.ngayDieuTri.Year == nam &&
        //                (x.bacSiDieuTriTayId == nv.nhanVienId ||
        //                 x.kyThuatVienTapId == nv.nhanVienId))
        //            .ToListAsync();

        //        decimal hoaHongDieuTri = 0;
        //        decimal hoaHongTap = 0;

        //        foreach (var b in buois)
        //        {
        //            var dot = await _context.DotDieuTris
        //                .FirstAsync(x => x.dotDieuTriId == b.dotDieuTriId);

        //            decimal heSoGiam =
        //                Math.Min(dot.phanTramGiamGia / 100m, 0.5m);

        //            if (b.bacSiDieuTriTayId == nv.nhanVienId)
        //                hoaHongDieuTri +=
        //                    cauHinh.tienDieuTriTayMoiBuoi * (1 - heSoGiam);

        //            if (b.kyThuatVienTapId == nv.nhanVienId)
        //                hoaHongTap +=
        //                    cauHinh.tienTapMoiBuoi * (1 - heSoGiam);
        //        }

        //        if (hoaHongDieuTri > 0)
        //            bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
        //            {
        //                loai = LoaiLuongChiTiet.HoaHongDieuTri,
        //                soTien = hoaHongDieuTri,
        //                dienGiai = "Hoa hong dieu tri tay"
        //            });

        //        if (hoaHongTap > 0)
        //            bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
        //            {
        //                loai = LoaiLuongChiTiet.HoaHongTap,
        //                soTien = hoaHongTap,
        //                dienGiai = "Hoa hong tap"
        //            });

        //        // ================= HOA HỒNG GIỚI THIỆU =================
        //        var dots = await _context.DotDieuTris
        //            .Join(_context.BenhNhans,
        //                  d => d.benhNhanId,
        //                  b => b.benhNhanId,
        //                  (d, b) => new { d, b })
        //            .Where(x =>
        //                x.b.nguoiGioiThieuId == nv.nhanVienId &&
        //                x.d.daThanhToan >= x.d.tongTien &&
        //                x.d.ngayThanhToan.HasValue &&
        //                x.d.ngayThanhToan.Value.Month == thang &&
        //                x.d.ngayThanhToan.Value.Year == nam)
        //            .ToListAsync();

        //        decimal hoaHongGioiThieu =
        //            dots.Sum(x => x.d.tongTien * cauHinh.phanTramGioiThieu / 100);

        //        if (hoaHongGioiThieu > 0)
        //            bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
        //            {
        //                loai = LoaiLuongChiTiet.HoaHongGioiThieu,
        //                soTien = hoaHongGioiThieu,
        //                dienGiai = "Hoa hong gioi thieu"
        //            });

        //        // ===============================
        //        // TÍNH TĂNG CA (OT)
        //        // ===============================

        //        // 1. LƯƠNG / GIỜ
        //        decimal luongMoiGio =
        //            nv.luongCoBan / (soNgayLamChuan * cauHinh.soGioLamChuanNgay);

        //        // 2. TỔNG OT
        //        decimal tongTienOT = 0;

        //        foreach (var cc in chamCongs)
        //        {
        //            var laNgayDacBiet =
        //                LuongTangCaHelper.LaNgayLeHoacChuNhat(
        //                    cc.thoiGianVao.Date,
        //                    ngayLes);

        //            tongTienOT += LuongTangCaHelper.TinhOTChoNgay(
        //                cc,
        //                cauHinh,
        //                luongMoiGio,
        //                laNgayDacBiet);
        //        }

        //        // 3. GHI BẢNG LƯƠNG
        //        if (tongTienOT > 0)
        //        {
        //            bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
        //            {
        //                loai = LoaiLuongChiTiet.TangCa,
        //                soTien = Math.Round(tongTienOT, 0),
        //                dienGiai = "Tien tang ca"
        //            });
        //        }

        //        bangLuong.tongLuong =
        //            bangLuong.ChiTiets.Sum(x => x.soTien);

        //        _context.BangLuongThangs.Add(bangLuong);
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}