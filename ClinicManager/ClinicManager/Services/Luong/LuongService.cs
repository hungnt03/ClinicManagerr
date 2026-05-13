using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.Utils;
using ClinicManager.ViewModels.Luong;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ClinicManager.Services.Luong
{
    public interface ILuongService
    {
        Task<int> TinhLuongThangAsync(int thang, int nam);
        Task ChotLuongAsync(int bangLuongThangId);
        Task MoChotLuongAsync(int bangLuongThangId);
        Task UpdateTongLuongNhanVienAsync(int bangLuongThangId);
        Task<List<ThuExcelVM>> TaoThuExcelAsync(int bangLuongThangId, DateTime stDate, DateTime edDate);
        Task<List<ChiExcelVM>> TaoChiExcelAsync(int bangLuongThangId, DateTime stDate, DateTime edDate);
        Task<List<ChamCongExcelVM>> TaoChamCongExcelAsync(int bangLuongThangId, DateTime stDate, DateTime edDate);
        Task<List<TheoDoiDieuTriExcelVM>> TaoTheoDoiDieuTriExcelVMAsync(int bangLuongThangId, DateTime stDate, DateTime edDate);
        Task<List<LuongExcelVM>> TaoLuongExcelVMAsync(int bangLuongThangId, DateTime stDate, DateTime edDate);
        Task<byte[]> TaoFileLuongExcelAsync(XuatExcelVM data, string templatePath);
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
            // Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            // 1.Kiểm tra bảng lương đã tồn tại chưa
            var bangLuong = await _context.BangLuongThangs
                .Include(x => x.ChiTiets)
                .FirstOrDefaultAsync(x => x.thang == thang && x.nam == nam);
            List<BangLuongThangChiTiet> backupPhatSinh = new List<BangLuongThangChiTiet>();

            try
            {
                if (bangLuong != null)
                {
                    if (bangLuong.daChot) throw new Exception("Bảng lương đã chốt, không thể tính lại");

                    // 🟢 BƯỚC 1: Backup các dòng Phát Sinh tay (Loai == PhatSinh)
                    backupPhatSinh = bangLuong.ChiTiets
                        .Where(x => x.loai == LoaiLuongChiTiet.PhatSinh)
                        .Select(x => new BangLuongThangChiTiet
                        {
                            nhanVienId = x.nhanVienId,
                            loai = x.loai,
                            soTien = x.soTien,
                            dienGiai = x.dienGiai
                        }).ToList();

                    // 🔴 BƯỚC 2: Xóa hết chi tiết cũ để tính lại từ đầu
                    _context.BangLuongThangChiTiets.RemoveRange(bangLuong.ChiTiets);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Nếu chưa có thì tạo mới bảng chính
                    bangLuong = new BangLuongThang { thang = thang, nam = nam, taoLuc = DateTime.Now, daChot = false };
                    _context.BangLuongThangs.Add(bangLuong);
                }

                //===================================================================================================================

                //// 1️. Không cho tính lại
                //if (await _context.BangLuongThangs
                //    .AnyAsync(x => x.thang == thang && x.nam == nam))
                //{
                //    throw new Exception($"Bảng lương tháng {thang}/{nam} đã tồn tại");
                //}

                // 2️. Cấu hình lương áp dụng
                var cauHinh = await _context.CauHinhLuongs
                    .Where(x => x.apDungTuNgay <= new DateTime(nam, thang, 1))
                    .OrderByDescending(x => x.apDungTuNgay)
                    .FirstAsync();

                // 3️. Ngày lễ
                var ngayLes = await _context.NgayLes
                    .Where(x => x.ngay.Month == thang && x.ngay.Year == nam)
                    .ToListAsync();

                // 4️⃣ Nhân viên đang hoạt động 
                var nhanViens = await _context.NhanViens
                    .Where(x => x.hoatDong)
                    .ToListAsync();

                //// 5️. Tạo bảng lương tháng (DUY NHẤT)
                //var bangLuong = new BangLuongThang
                //{
                //    thang = thang,
                //    nam = nam,
                //    taoLuc = DateTime.Now,
                //    daChot = false
                //};

                // 6️. Tính số ngày làm chuẩn
                int soNgayTrongThang = DateTime.DaysInMonth(nam, thang);
                int soChuNhat = Enumerable.Range(1, soNgayTrongThang)
                    .Count(d => new DateTime(nam, thang, d).DayOfWeek == DayOfWeek.Sunday);

                // Số ngày lễ có tính lương
                int soNgayLeTinhLuong = ngayLes.Count(x => x.coTinhLuong);

                int soNgayLamChuan =
                    soNgayTrongThang - soChuNhat - soNgayLeTinhLuong;

                // Tính toán thời gian nghỉ trưa từ cấu hình (Ví dụ: 13:30 - 12:00 = 1.5h)
                double gioNghiTrua = (cauHinh.gioBatDauChieu - cauHinh.gioKetThucSang).TotalHours;
                double gioCongCanThietChoAnTrua = 8.0 + gioNghiTrua;

                // 7️. Duyệt từng nhân viên
                foreach (var nv in nhanViens)
                {
                    // ===== CHẤM CÔNG =====
                    var chamCongs = await _context.ChamCongs
                        .Where(x =>
                            x.nhanVienId == nv.nhanVienId &&
                            x.thoiGianVao.Month == thang &&
                            x.thoiGianVao.Year == nam)
                        .ToListAsync();

                    var buoiDieuTris = await _context.BuoiDieuTris
                                                .Where(b => (b.bacSiDieuTriTayId == nv.nhanVienId || b.kyThuatVienTapId == nv.nhanVienId)
                                                    && b.ngayDieuTri.Month == thang && b.ngayDieuTri.Year == nam)
                                                .Join(_context.DotDieuTris, b => b.dotDieuTriId, d => d.dotDieuTriId, (b, d) => new
                                                {
                                                    bacSiDieuTriTayId = b.bacSiDieuTriTayId,
                                                    kyThuatVienTapId = b.kyThuatVienTapId,
                                                    ngayDieuTri = b.taoLuc,
                                                    phanTramGiamGia = d.phanTramGiamGia
                                                }).ToListAsync();

                    int soNgayLamThucTe = chamCongs.Count(x => !x.nghiPhep);
                    decimal donGiaNgayCong = nv.luongCoBan / soNgayLamChuan;

                    // A. LƯƠNG CƠ BẢN (Theo ngày làm)
                    decimal luongCoBan = donGiaNgayCong * soNgayLamThucTe;

                    bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                    {
                        nhanVienId = nv.nhanVienId,
                        loai = LoaiLuongChiTiet.LuongCoBan,
                        soTien = Math.Round(luongCoBan, 0),
                        dienGiai = $"Lương cơ bản ({soNgayLamThucTe}/{soNgayLamChuan} ngày)"
                    });

                    // B. LƯƠNG NGÀY LỄ (Hưởng 100%)
                    if (soNgayLeTinhLuong > 0)
                    {
                        decimal luongNgayLe = donGiaNgayCong * soNgayLeTinhLuong;
                        bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                        {
                            nhanVienId = nv.nhanVienId,
                            loai = LoaiLuongChiTiet.LuongNgayLe,
                            soTien = Math.Round(luongNgayLe, 0),
                            dienGiai = $"Lương ngày lễ ({soNgayLeTinhLuong} ngày)"
                        });
                    }

                    // C. XĂNG XE (Tính theo tỷ lệ ngày công thực tế)
                    decimal phuCapXang = cauHinh.tienXangXeThang * soNgayLamThucTe / soNgayLamChuan;
                    bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                    {
                        nhanVienId = nv.nhanVienId,
                        loai = LoaiLuongChiTiet.XangXe,
                        soTien = Math.Round(phuCapXang, 0),
                        dienGiai = $"Xăng xe theo ngày công ({soNgayLamThucTe}/{soNgayLamChuan} ngày)"
                    });

                    // D. ĂN TRƯA (Điều kiện >= 9.5h tùy cấu hình (8h làm việc + 1.5h nghỉ trưa))
                    int soNgayAnTrua = chamCongs.Count(x => x.anTrua);
                    int soBuoiAnTrua = chamCongs.Count(x => x.anTrua && x.thoiGianRa.HasValue &&
                                                            (x.thoiGianRa.Value - x.thoiGianVao).TotalHours >= gioCongCanThietChoAnTrua);

                    if (soNgayAnTrua > 0)
                    {
                        bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                        {
                            nhanVienId = nv.nhanVienId,
                            loai = LoaiLuongChiTiet.AnTrua,
                            soTien = soNgayAnTrua * cauHinh.tienAnTruaNgay,
                            dienGiai = $"Ăn trưa ({soNgayAnTrua} ngày đủ >={gioCongCanThietChoAnTrua}h)"
                        });
                    }

                    // E. HOA HỒNG (Tính theo từng buổi + % giảm giá đợt)
                    // Điều trị tay
                    decimal tongHoaHong = 0;
                    var buoiDieuTrisDieuTriTay = buoiDieuTris.Where(b => b.bacSiDieuTriTayId == nv.nhanVienId).ToList();
                    foreach (var b in buoiDieuTrisDieuTriTay)
                    {
                        decimal tienGocBuoi = 0;
                        if (b.bacSiDieuTriTayId == nv.nhanVienId) tienGocBuoi += cauHinh.tienDieuTriTayMoiBuoi;

                        decimal heSoConLai = 1 - ((b.phanTramGiamGia) / 100m);
                        tongHoaHong += tienGocBuoi * heSoConLai;
                    }

                    if (tongHoaHong > 0)
                    {
                        bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                        {
                            nhanVienId = nv.nhanVienId,
                            loai = LoaiLuongChiTiet.HoaHongDieuTriTay,
                            soTien = Math.Round(tongHoaHong, 0),
                            dienGiai = $"Hoa hồng điều trị tay({buoiDieuTrisDieuTriTay.Count} buổi)"
                        });
                    }

                    // Tập
                    tongHoaHong = 0;
                    var BuoiDieuTrisTap = buoiDieuTris.Where(b => b.kyThuatVienTapId == nv.nhanVienId).ToList();
                    foreach (var b in BuoiDieuTrisTap)
                    {
                        decimal tienGocBuoi = 0;
                        if (b.kyThuatVienTapId == nv.nhanVienId) tienGocBuoi += cauHinh.tienTapMoiBuoi;

                        decimal heSoConLai = 1 - ((b.phanTramGiamGia) / 100m);
                        tongHoaHong += tienGocBuoi * heSoConLai;
                    }

                    if (tongHoaHong > 0)
                    {
                        bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                        {
                            nhanVienId = nv.nhanVienId,
                            loai = LoaiLuongChiTiet.HoaHongTap,
                            soTien = Math.Round(tongHoaHong, 0),
                            dienGiai = $"Hoa hồng tập ({BuoiDieuTrisTap.Count} buổi)"
                        });
                    }

                    // Giới thiệu (Chỉ đợt đầu tiên của bệnh nhân)
                    tongHoaHong = 0;
                    //TODO
                    //var dotDieuTris = _context.DotDieuTris.Where(x => x.ngayKham.Year == nam && x.ngayKham.Month == thang).ToList();
                    //var benhNhanIds = dotDieuTris.Select(x => x.benhNhanId).Distinct().ToList();
                    //if (_context.BenhNhans.Any(x=>benhNhanIds.Contains(x.benhNhanId) && x.nguoiGioiThieuId == nv.nhanVienId))
                    //{

                    //}


                    //// lấy đợt điều trị đầu tiên trong lịch sử của bệnh nhân
                    //var dotDauTien = await _context.DotDieuTris
                    //    .Where(x => x.benhNhanId == d.benhNhanId)
                    //    .OrderBy(x => x.ngayKham) // Sắp xếp theo ngày khám tăng dần
                    //    .FirstOrDefaultAsync();



                    // --- Xác định mốc thời gian chặn (Ngày cuối cùng của tháng tính lương) ---
                    DateTime ngayCuoiThang = new DateTime(nam, thang, DateTime.DaysInMonth(nam, thang), 23, 59, 59);
                    // 1. Tìm tất cả các đợt điều trị được tạo trong tháng/nam này mà nhân viên này là người giới thiệu
                    // Lưu ý: Ta join với bảng BenhNhan để xác định đúng người giới thiệu
                    var dotDieuTrisTrongThang = await _context.DotDieuTris
                        .Join(_context.BenhNhans,
                              d => d.benhNhanId,
                              b => b.benhNhanId,
                              (d, b) => new
                              {
                                  benhNhanId = b.benhNhanId,
                                  nguoiGioiThieuId = b.nguoiGioiThieuId,
                                  tongTien = d.tongTien,
                                  ngayTao = d.ngayKham
                              })
                        .Where(x => x.nguoiGioiThieuId == nv.nhanVienId
                                 && x.ngayTao.Month == thang
                                 && x.ngayTao.Year == nam)
                        .ToListAsync();

                    foreach (var d in dotDieuTrisTrongThang)
                    {
                        // 2. ĐẾM SỐ ĐỢT CỦA BỆNH NHÂN NHƯNG CHỈ TÍNH ĐẾN HẾT THÁNG ĐANG TÍNH LƯƠNG
                        // Điều này đảm bảo nếu tháng sau BN quay lại đợt 2, khi tính lại lương tháng này vẫn đếm ra 1 đợt.
                        // Nếu Count == 1, nghĩa là đợt hiện tại là đợt duy nhất (đợt đầu tiên)
                        int soLuongDotCuaBenhNhan = await _context.DotDieuTris
                            .CountAsync(x => x.benhNhanId == d.benhNhanId && x.ngayKham <= ngayCuoiThang);

                        if (soLuongDotCuaBenhNhan == 1)
                        {
                            // Tính hoa hồng dựa trên tổng tiền của đợt đầu tiên
                            decimal tienHoaHong = d.tongTien * (cauHinh.phanTramGioiThieu / 100m);
                            tongHoaHong += tienHoaHong;
                        }
                    }

                    if (tongHoaHong > 0)
                    {
                        bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                        {
                            nhanVienId = nv.nhanVienId,
                            loai = LoaiLuongChiTiet.HoaHongGioiThieu,
                            soTien = Math.Round(tongHoaHong, 0),
                            dienGiai = $"Hoa hồng giới thiệu BN mới (Tính trên đợt đầu tiên của BN tính đến {thang}/{nam})"
                        });
                    }

                    // F. CHUYÊN CẦN (Làm đủ số ngày chuẩn)
                    if (soNgayLamThucTe >= soNgayLamChuan)
                    {
                        bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                        {
                            nhanVienId = nv.nhanVienId,
                            loai = LoaiLuongChiTiet.ChuyenCan,
                            soTien = cauHinh.tienChuyenCan,
                            dienGiai = "Chuyên cần (Đạt chuẩn công)"
                        });
                    }

                    // G. TĂNG CA (OT)
                    decimal tongOT = 0;
                    foreach (var cc in chamCongs)
                    {
                        tongOT += LuongTangCaHelper.TinhOTChoNgay(cc, cauHinh);
                    }

                    if (tongOT > 0)
                    {
                        bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                        {
                            nhanVienId = nv.nhanVienId,
                            loai = LoaiLuongChiTiet.TangCa,
                            soTien = tongOT,
                            dienGiai = "Tăng ca (sau 16h, thời gian >= 30 phút)"
                        });
                    }

                    //Sau khi tính xong các khoản tự động, nạp lại phần backup
                    foreach (var ps in backupPhatSinh)
                    {
                        bangLuong.ChiTiets.Add(ps);
                    }

                    //// H. PHÁT SINH (ADMIN ONLY) - Tính sau khi tính xong các khoản trên
                    //var phatSinhs = await _context.PhatSinhLuongs
                    //                                .Where(x => x.nhanVienId == nv.nhanVienId
                    //                                         && x.taoLuc.Month == thang
                    //                                         && x.taoLuc.Year == nam)
                    //                                .ToListAsync();
                    //foreach (var ps in phatSinhs)
                    //{
                    //    bangLuong.ChiTiets.Add(new BangLuongThangChiTiet
                    //    {
                    //        nhanVienId = nv.nhanVienId,
                    //        loai = LoaiLuongChiTiet.PhatSinh, // Loại phát sinh
                    //        soTien = ps.loai == LoaiPhatSinhLuong.Tru ? -ps.soTien : ps.soTien, // Nếu là phạt thì lưu số âm
                    //        dienGiai = ps.noiDung
                    //    });
                    //}
                }
                // 8️. Tổng lương tháng
                bangLuong.tongLuong = bangLuong.ChiTiets.Sum(x => x.soTien);
                //_context.BangLuongThangs.Add(bangLuong);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return bangLuong.bangLuongThangId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
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

        public async Task UpdateTongLuongNhanVienAsync(int bangLuongThangId)
        {
            // 1. Chỉ lấy bảng lương nếu chưa chốt
            var bangLuong = await _context.BangLuongThangs
                .FirstOrDefaultAsync(x => x.bangLuongThangId == bangLuongThangId && !x.daChot);

            if (bangLuong == null) return;

            // 2. Tính tổng trực tiếp từ Database (không nạp hết vào RAM)
            // Cách này nhanh hơn rất nhiều khi dữ liệu lớn
            decimal tongMoi = await _context.BangLuongThangChiTiets
                .Where(x => x.bangLuongThangId == bangLuongThangId)
                .SumAsync(x => x.soTien);

            // 3. Cập nhật và lưu
            bangLuong.tongLuong = tongMoi;
            await _context.SaveChangesAsync();
        }

        public async Task<List<ThuExcelVM>> TaoThuExcelAsync(int bangLuongThangId, DateTime stDate, DateTime edDate)
        {
            // 1. Thu
            var results = await (from d in _context.DotDieuTris
                          join b in _context.BenhNhans on d.benhNhanId equals b.benhNhanId
                          join g in _context.GoiDieuTris on d.goiDieuTriId equals g.goiDieuTriId
                          where d.ngayThanhToan >= stDate && d.ngayThanhToan <= edDate
                          orderby d.ngayThanhToan
                          select new ThuExcelVM
                          {
                              DotDieuTriId = d.dotDieuTriId,
                              NgayThang = d.ngayThanhToan.Value,
                              HoTen = b.hoTen,
                              GoiDieuTri = g.tenGoi,
                              GiamGia = d.phanTramGiamGia,
                              ThanhTien = d.tongTien,
                              GhiChu = $"{(d.daThanhToan == d.tongTien ? "Thu đủ" : (d.daThanhToan > 0 ? "Thanh toán 1 phần" : "Chưa thanh toán"))} - {(d.trangThai == TrangThaiDotDieuTri.HoanThanh? "Điều trị kết thúc" : "Đang điều trị")}"
                          }).ToListAsync();
            var dotDieuTriIdList = results.Select(x => x.DotDieuTriId).ToList();
            // 1. Lấy toàn bộ dữ liệu của các đợt trong danh sách
            var rawData = await _context.ThuocVatTuBuoiDieuTris
                                            .Where(tvt => dotDieuTriIdList.Contains(tvt.BuoiDieuTri.dotDieuTriId))
                                            .Select(tvt => new
                                            {
                                                DotId = tvt.BuoiDieuTri.dotDieuTriId,
                                                TenVatTu = tvt.VatTu.tenVatTu,
                                                ThanhTien = tvt.soLuong * tvt.donGia
                                            })
                                            .ToListAsync();
            // 2. Nhóm dữ liệu và xử lý chuỗi trên bộ nhớ
            var thongKe = rawData
                                .GroupBy(x => x.DotId)
                                .Select(g => new
                                {
                                    DotDieuTriId = g.Key,
                                    TongTienThuoc = g.Sum(x => x.ThanhTien),
                                    // Lấy danh sách tên không trùng, sắp xếp và nối chuỗi
                                    DanhSachVatTu = string.Join(", ", g.Select(x => x.TenVatTu)
                                                                       .Distinct()
                                                                       .OrderBy(n => n))
                                }).ToList();
            foreach (var item in thongKe)
            {
                results.FirstOrDefault(x => x.DotDieuTriId == item.DotDieuTriId).TenThuoc = item.DanhSachVatTu;
                results.FirstOrDefault(x => x.DotDieuTriId == item.DotDieuTriId).TienThuoc = item.TongTienThuoc;
            }

            return results;
        }

        public Task<List<ChiExcelVM>> TaoChiExcelAsync(int bangLuongThangId, DateTime stDate, DateTime edDate)
        {
            var results = from ct in _context.BangLuongThangChiTiets
                          join nv in _context.NhanViens on ct.nhanVienId equals nv.nhanVienId
                          where ct.bangLuongThangId == bangLuongThangId && ct.loai == LoaiLuongChiTiet.PhatSinh
                          select new ChiExcelVM
                          {
                              NgayThang = ct.dienGiai.ExtractDate() ?? edDate,
                              TenKhoanChi = $"{ct.loai} - {ct.dienGiai}",
                              ThanhTien = ct.soTien,
                              GhiChu = nv.hoTen
                          };
            return results.ToListAsync();
        }

        public Task<List<ChamCongExcelVM>> TaoChamCongExcelAsync(int bangLuongThangId, DateTime stDate, DateTime edDate)
        {
            var cauHinh = _context.CauHinhLuongs
                .Where(x => x.apDungTuNgay <= stDate)
                .OrderByDescending(x => x.apDungTuNgay)
                .FirstOrDefault();

            var result = from cc in _context.ChamCongs
                         join nv in _context.NhanViens on cc.nhanVienId equals nv.nhanVienId
                         where cc.thoiGianVao >= stDate && cc.thoiGianVao <= edDate
                         select new ChamCongExcelVM
                         {
                             NhanVienId = nv.nhanVienId,
                             HoTen = nv.hoTen,
                             Checkin = cc.thoiGianVao,
                             Checkout = cc.thoiGianRa,
                             DiLam = !cc.nghiPhep,
                             TangCa = LuongTangCaHelper.GetGioOT(cc, cauHinh),
                             TienTangCa = LuongTangCaHelper.TinhOTChoNgay(cc, cauHinh) * cauHinh.donGiaTangCaMoiGio,
                             ComTrua = cc.anTrua
                         };
            return result.OrderBy(x => x.NhanVienId).ToListAsync();
        }

        public async Task<List<TheoDoiDieuTriExcelVM>> TaoTheoDoiDieuTriExcelVMAsync(int bangLuongThangId, DateTime stDate, DateTime edDate)
        {
            var results = (from b in _context.BuoiDieuTris
                          join bn in _context.BenhNhans on b.benhNhanId equals bn.benhNhanId
                          join d in _context.DotDieuTris on b.dotDieuTriId equals d.dotDieuTriId
                           where b.ngayDieuTri >= stDate && b.ngayDieuTri <= edDate
                          select new TheoDoiDieuTriExcelVM
                          {
                              BenhNhanId = bn.benhNhanId,
                              HoTenBenhNhan = bn.hoTen,
                              NgayThang = b.ngayDieuTri,
                              DieuTriTay = b.bacSiDieuTriTayId.HasValue,
                              BsDieuTriTayId = b.bacSiDieuTriTayId,
                              DieuTriTap = b.kyThuatVienTapId.HasValue,
                              BsDieuTriTapId = b.kyThuatVienTapId,
                              HoanThanh = d.trangThai == TrangThaiDotDieuTri.HoanThanh
                          }).ToList().ToList();

            var nhanViens = _context.NhanViens.ToList();
            foreach (var item in results)
            {
                if (item.DieuTriTay)
                {
                    item.TenBsDieuTriTay = nhanViens.FirstOrDefault(nv => nv.nhanVienId == item.BsDieuTriTayId.Value).hoTen;
                }
                if (item.DieuTriTap)
                {
                    item.TenBsDieuTriTap = nhanViens.FirstOrDefault(nv => nv.nhanVienId == item.BsDieuTriTapId.Value).hoTen;
                }
            }

            return results;
        }

        public Task<List<LuongExcelVM>> TaoLuongExcelVMAsync(int bangLuongThangId, DateTime stDate, DateTime edDate)
        {
            return  (from l in _context.BangLuongThangChiTiets
                                join nv in _context.NhanViens on l.nhanVienId equals nv.nhanVienId
                                where l.bangLuongThangId == bangLuongThangId
                                // Nhóm theo các thông tin chung của nhân viên
                                group l by new { nv.nhanVienId, nv.hoTen } into g
                                select new LuongExcelVM
                                {
                                    NhanVienId = g.Key.nhanVienId,
                                    HoTen = g.Key.hoTen,
                                    // Toàn bộ chi tiết lương thuộc về nhân viên này sẽ được đưa vào List
                                    LuongChiTietList = g.Select(x => new LuongChiTietExcelVM
                                    {
                                        Loai = x.loai,
                                        SoTien = x.soTien,
                                        GhiChu = x.dienGiai
                                    }).ToList()
                                }).ToListAsync();
        }

        public async Task<byte[]> TaoFileLuongExcelAsync(XuatExcelVM data, string templatePath)
        {
            var fileInfo = new FileInfo(templatePath);
            using (var package = new ExcelPackage(fileInfo))
            {
                // Thu
                var ws = package.Workbook.Worksheets["Thu"];
                int startRow = 3;
                int currRow = startRow;
                int lastCol = ws.Dimension.End.Column;

                ws.Cells[1, 1].Value = $"BẢNG THU TIỀN THÁNG {data.NgayThang.Month} NĂM {data.NgayThang.Year}";
                foreach (var item in data.ThuList)
                {
                    // Từ dòng thứ 5 trở đi thì insert thêm row
                    if (currRow >= 5) 
                    {
                        ws.InsertRow(currRow, 1, currRow - 1);
                        ws.Cells[currRow - 1, 1, currRow - 1, lastCol].Copy(ws.Cells[currRow, 1, currRow, lastCol]);
                    }
                    ws.Cells[currRow, 1].Value = item.NgayThang;
                    ws.Cells[currRow, 2].Value = item.HoTen;
                    ws.Cells[currRow, 3].Value = item.GoiDieuTri;
                    ws.Cells[currRow, 4].Value = item.GiamGia;
                    ws.Cells[currRow, 5].Value = item.ThanhTien;
                    ws.Cells[currRow, 7].Value = item.TienThuoc;
                    ws.Cells[currRow, 8].Value = item.TenThuoc;
                    ws.Cells[currRow, 10].Value = item.GhiChu;

                    currRow += 1;
                }

                // Chi
                ws = package.Workbook.Worksheets["Chi"];
                startRow = 3;
                currRow = startRow;
                foreach (var item in data.ChiList)
                {
                    // Từ dòng thứ 5 trở đi thì insert thêm row
                    if (currRow >= 5) 
                    {
                        ws.InsertRow(currRow, 1);
                        ws.Cells[currRow - 1, 1, currRow - 1, lastCol].Copy(ws.Cells[currRow, 1, currRow, lastCol]);
                    }
                    ws.Cells[currRow, 1].Value = new DateTime(item.NgayThang.Year, item.NgayThang.Month, item.NgayThang.Day);
                    ws.Cells[currRow, 2].Value = item.TenKhoanChi;
                    ws.Cells[currRow, 3].Value = item.ThanhTien;
                    ws.Cells[currRow, 4].Value = item.GhiChu;

                    currRow += 1;
                }

                // Chấm công
                ws = package.Workbook.Worksheets["Cham cong"];
                startRow = 9;
                var currCol = 3;
                currRow = startRow;

                ws.Cells[1, 2].Value = data.NgayThang.Month;
                ws.Cells[2, 2].Value = data.NgayThang.Year;
                var nhanviens = _context.NhanViens.Where(x => x.hoatDong).ToList();
                foreach (var nv in nhanviens)
                {
                    var id = nv.nhanVienId;

                    ws.Cells[currRow, 1].Value = nv.hoTen;
                    var chamCongSubList = data.ChamCongList.Where(x => x.NhanVienId == id).OrderBy(x => x.Checkin).ToList();
                    
                    foreach (var item in chamCongSubList)
                    {
                        if (item.Checkin.HasValue)
                        {
                            var congCell = ws.Cells[currRow, currCol - 1 + item.Checkin.Value.Day];
                            congCell.Value = "x";
                            if ((item.Checkout.Value - item.Checkin.Value).TotalHours < Convert.ToDouble(data.cfg.soGioLamChuanNgay) + (data.cfg.gioBatDauChieu - data.cfg.gioKetThucChieu).TotalHours)
                            {
                                congCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                congCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                            }

                            if (item.TangCa > 0) ws.Cells[currRow + 1, currCol - 1 + item.Checkin.Value.Day].Value = item.TangCa;
                            if (item.ComTrua) ws.Cells[currRow + 2, currCol - 1 + item.Checkin.Value.Day].Value = "x";

                        }
                    }

                    currRow += 3;
                }

                // Theo dõi điều trị
                ws = package.Workbook.Worksheets["Theo doi dieu tri"];
                startRow = 9;
                currCol = 2;
                currRow = startRow;

                ws.Cells[1, 2].Value = data.NgayThang.Month;
                ws.Cells[2, 2].Value = data.NgayThang.Year;
                ws.Cells[4, 3].Value = $"BẢNG THEO DÕI NGÀY LÀM VIỆC TRONG THÁNG {data.NgayThang.Month}/{data.NgayThang.Year}";

                var benhNhanIdList = data.TheoDoiDieuTriList.Select(x => x.BenhNhanId).Distinct().ToList();
                var benhNhans = await _context.BenhNhans.Where(x => benhNhanIdList.Contains(x.benhNhanId)).ToListAsync();

                foreach (var bn in benhNhans)
                {
                    var id = bn.benhNhanId;
                    ws.Cells[currRow, 2].Value = bn.hoTen;

                    var TheoDoiDieuTriSubList = data.TheoDoiDieuTriList.Where(x => x.BenhNhanId == id).ToList();
                    foreach (var item in TheoDoiDieuTriSubList) 
                    {
                        // Ca sáng
                        if (item.NgayThang.Hour <= data.cfg.gioKetThucSang.TotalHours)
                        {
                            if (item.BsDieuTriTayId.HasValue) ws.Cells[currRow + 1, currCol + (item.NgayThang.Day * 2) - 1].Value = item.TenBsDieuTriTay;
                            if (item.BsDieuTriTapId.HasValue) ws.Cells[currRow + 2, currCol + (item.NgayThang.Day * 2) - 1].Value = item.TenBsDieuTriTap;

                            var cell = ws.Cells[currRow, currCol + (item.NgayThang.Day * 2) - 1];
                            if (item.HoanThanh)
                            {
                                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Lime);
                            }
                            else
                            {
                                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SandyBrown);
                            }
                        }
                        // Ca chiều
                        else
                        {
                            if (item.BsDieuTriTayId.HasValue) ws.Cells[currRow + 1, currCol + (item.NgayThang.Day * 2)].Value = item.TenBsDieuTriTay;
                            if (item.BsDieuTriTapId.HasValue) ws.Cells[currRow + 2, currCol + (item.NgayThang.Day * 2)].Value = item.TenBsDieuTriTap;

                            var cell = ws.Cells[currRow, currCol + (item.NgayThang.Day * 2)];
                            if (item.HoanThanh)
                            {
                                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Lime);
                            }
                            else
                            {
                                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SandyBrown);
                            }
                        }
                    }

                    currRow += 3;
                }

                //foreach (var item in data.TheoDoiDieuTriList)
                //{
                //    ws.Cells[currRow, 2].Value = item.HoTenBenhNhan;
                //    // TODO
                //    // Ca sáng
                //    if (item.NgayThang.Hour <= data.cfg.gioKetThucSang.TotalHours)
                //    {
                //        if (item.BsDieuTriTayId.HasValue) ws.Cells[currRow + 1, currCol - 1 + item.NgayThang.Day].Value = item.TenBsDieuTriTay;
                //        if (item.BsDieuTriTapId.HasValue) ws.Cells[currRow + 2, currCol - 1 + item.NgayThang.Day].Value = item.TenBsDieuTriTap;
                //    }
                //    // Ca chiều
                //    else
                //    {
                //        if (item.BsDieuTriTayId.HasValue) ws.Cells[currRow + 1, currCol + item.NgayThang.Day].Value = item.TenBsDieuTriTay;
                //        if (item.BsDieuTriTapId.HasValue) ws.Cells[currRow + 2, currCol + item.NgayThang.Day].Value = item.TenBsDieuTriTap;
                //    }

                //    currRow += 3;
                //}

                // Bảng lương
                // Clone
                foreach (var item in data.LuongList)
                {
                    ws = package.Workbook.Worksheets["Luong"];
                    package.Workbook.Worksheets.Add($"Luong_{item.HoTen}", ws);
                    ws = package.Workbook.Worksheets[$"Luong_{item.HoTen}"];

                    ws.Cells[1, 1].Value = $"BẢNG LƯƠNG {item.HoTen} THÁNG {data.NgayThang.Month} NĂM {data.NgayThang.Year}";
                    currRow = 2;
                    lastCol = ws.Dimension.End.Column;

                    foreach (var detail in item.LuongChiTietList.OrderBy(x => x.Loai))
                    {
                        // Từ dòng thứ 5 trở đi thì insert thêm row
                        if (currRow >= 5) 
                        {
                            ws.InsertRow(currRow, 1, currRow - 1);
                            ws.Cells[currRow - 1, 1, currRow - 1, lastCol].Copy(ws.Cells[currRow, 1, currRow, lastCol]);
                        }
                        ws.Cells[currRow, 1].Value = GetTenLoaiLuong(detail.Loai);
                        ws.Cells[currRow, 2].Value = detail.SoTien;
                        ws.Cells[currRow, 3].Value = detail.GhiChu;

                        currRow += 1;
                    }
                }

                // Hidden Luong sheet
                ws = package.Workbook.Worksheets["Luong"];
                ws.Hidden = eWorkSheetHidden.Hidden;

                return package.GetAsByteArray();
            }
        }

        // Hàm Helper để hiển thị tên loại lương tiếng Việt cho đẹp
        private string GetTenLoaiLuong(LoaiLuongChiTiet loai)
        {
            switch (loai)
            {
                case LoaiLuongChiTiet.LuongCoBan: return "Lương cơ bản";
                case LoaiLuongChiTiet.AnTrua: return "Phụ cấp ăn trưa";
                case LoaiLuongChiTiet.XangXe: return "Phụ cấp xăng xe";
                case LoaiLuongChiTiet.HoaHongDieuTriTay: return "Hoa hồng điều trị tay";
                case LoaiLuongChiTiet.HoaHongTap: return "Hoa hồng tập";
                case LoaiLuongChiTiet.TangCa: return "Tăng ca";
                case LoaiLuongChiTiet.PhatSinh: return "Phát sinh (Thưởng/Phạt)";
                case LoaiLuongChiTiet.HoaHongGioiThieu: return "Hoa hồng giới thiệu";
                default: return "Khác";
            }
        }
    }
}