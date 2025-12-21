using ClinicManager.Models.Entities;

namespace ClinicManager.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            //// Nếu đã có gói mặc định thì bỏ qua
            //if (context.GoiDieuTris.Any(x => x.macDinh))
            //    return;

            //var goiMacDinh = new GoiDieuTri
            //{
            //    tenGoi = "Goi 1 buoi",
            //    soBuoi = 1,
            //    gia = 200000, // chỉnh theo phòng khám
            //    hoatDong = true
            //};

            //context.GoiDieuTris.Add(goiMacDinh);
            //context.SaveChanges();

            if (!context.CauHinhLuongs.Any())
            {
                context.CauHinhLuongs.Add(new CauHinhLuong
                {
                    gioBatDauSang = new TimeSpan(8, 0, 0),
                    gioKetThucSang = new TimeSpan(12, 0, 0),
                    gioBatDauChieu = new TimeSpan(13, 30, 0),
                    gioKetThucChieu = new TimeSpan(17, 30, 0),

                    soGioLamChuanNgay = 8,

                    tienAnTruaNgay = 30000,
                    tienXangXeThang = 500000,
                    tienChuyenCan = 200000,

                    tienDieuTriTayMoiBuoi = 30000,
                    tienTapMoiBuoi = 20000,
                    phanTramGioiThieu = 5,

                    heSoTangCaNgayThuong = 1.25m,
                    heSoTangCaNgayLe = 1.5m,

                    soPhutLamTronTangCa = 30,
                    soPhutToiThieuTinhTangCa = 60,

                    apDungTuNgay = new DateTime(2025, 1, 1),
                    taoLuc = DateTime.Now
                });

                context.SaveChanges();
            }

        }
    }
}
