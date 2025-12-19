using ClinicManager.Data;
using ClinicManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    /// <summary>
    /// ❌ Tạo khám mà không tạo gói
    ////❌ UI chọn gói nhưng backend không ép
    ////❌ Cho 1 bệnh nhân khám nhiều lần 1 ngày
    ////❌ Đổi gói sau khi đã điều trị
    ////❌ Không dùng transaction
    /// </summary>
    public class KhamBenhService : IKhamBenhService
    {
        private readonly ApplicationDbContext _context;

        public KhamBenhService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kiểm tra bệnh nhân đã có khám trong ngày chưa
        public async Task<bool> CoKhamTrongNgayAsync(int benhNhanId, DateTime ngay)
        {
            return await _context.KhamBenhs.AnyAsync(x =>
                x.benhNhanId == benhNhanId &&
                x.ngayKham.Date == ngay.Date
            );
        }

        // Tạo khám bệnh – ÉP NGHIỆP VỤ
        public async Task<KhamBenh> TaoKhamBenhAsync(
            int benhNhanId,
            int bacSiId,
            string chanDoan,
            string phacDoDieuTri,
            int? goiDieuTriId = null
        )
        {
            // 1. Kiểm tra bệnh nhân tồn tại
            var benhNhan = await _context.BenhNhans
                .FirstOrDefaultAsync(x => x.benhNhanId == benhNhanId);

            if (benhNhan == null)
                throw new Exception("Benh nhan khong ton tai");

            // 2. Không cho 1 ngày khám 2 lần
            if (await CoKhamTrongNgayAsync(benhNhanId, DateTime.Now))
                throw new Exception("Benh nhan da duoc kham trong ngay");

            // 3. Lấy gói điều trị
            GoiDieuTri goi;

            if (goiDieuTriId.HasValue)
            {
                goi = await _context.GoiDieuTris
                    .FirstOrDefaultAsync(x =>
                        x.goiDieuTriId == goiDieuTriId &&
                        x.hoatDong);

                if (goi == null)
                    throw new Exception("Goi dieu tri khong hop le");
            }
            else
            {
                goi = await _context.GoiDieuTris
                    .FirstOrDefaultAsync(x => x.hoatDong);

                if (goi == null)
                    throw new Exception("Khong tim thay goi dieu tri mac dinh");
            }

            // 4. Transaction để tránh dữ liệu lửng
            using var tran = await _context.Database.BeginTransactionAsync();

            try
            {
                // 5. Tạo khám bệnh
                var khamBenh = new KhamBenh
                {
                    benhNhanId = benhNhanId,
                    bacSiId = bacSiId,
                    ngayKham = DateTime.Now,
                    chanDoan = chanDoan,
                    phacDoDieuTri = phacDoDieuTri,
                    goiDieuTriId = goi.goiDieuTriId,
                    tinhTrangThanhToan = "ChuaThu",
                    taoLuc = DateTime.Now
                };

                _context.KhamBenhs.Add(khamBenh);
                await _context.SaveChangesAsync();

                // 6. Tạo gói bệnh nhân
                var benhNhanGoi = new BenhNhanGoiDieuTri
                {
                    benhNhanId = benhNhanId,
                    khamBenhId = khamBenh.khamBenhId,
                    goiDieuTriId = goi.goiDieuTriId,
                    tongSoBuoi = goi.soBuoi,
                    soBuoiDaDung = 0,
                    ngayMua = DateTime.Now,
                    trangThai = "DangSuDung"
                };

                _context.BenhNhanGoiDieuTris.Add(benhNhanGoi);
                await _context.SaveChangesAsync();

                await tran.CommitAsync();

                return khamBenh;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }
    }

    public interface IKhamBenhService
    {
        Task<KhamBenh> TaoKhamBenhAsync(
            int benhNhanId,
            int bacSiId,
            string chanDoan,
            string phacDoDieuTri,
            int? goiDieuTriId = null
        );

        Task<bool> CoKhamTrongNgayAsync(int benhNhanId, DateTime ngay);
    }
}
