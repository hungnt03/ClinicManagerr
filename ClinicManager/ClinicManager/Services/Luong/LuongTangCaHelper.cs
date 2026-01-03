using ClinicManager.Models.Entities;

namespace ClinicManager.Services.Luong
{
    public static class LuongTangCaHelper
    {
        public static bool LaNgayLeHoacChuNhat(
        DateTime ngay,
        List<NgayLe> ngayLes)
        {
            if (ngay.DayOfWeek == DayOfWeek.Sunday)
                return true;

            return ngayLes.Any(x => x.ngay.Date == ngay.Date);
        }

        public static decimal LamTronGioOT(
            int tongPhut,
            int buocLamTron)
        {
            if (tongPhut < buocLamTron)
                return 0;

            return Math.Floor(
                (decimal)tongPhut / buocLamTron
            ) * (buocLamTron / 60m);
        }

        // Tính OT cho 1 ngày
        public static decimal TinhOTChoNgay(
            ChamCong cc,
            CauHinhLuong cfg)
        {
            if (!cc.thoiGianRa.HasValue)
                return 0;

            var gioKetThucChuan =
                cc.thoiGianVao.Date + cfg.gioKetThucChieu;

            if (cc.thoiGianRa.Value <= gioKetThucChuan)
                return 0;

            int phutOT =
                (int)(cc.thoiGianRa.Value - gioKetThucChuan)
                .TotalMinutes;

            var gioOT = LamTronGioOT(
                phutOT,
                cfg.soPhutLamTronTangCa);

            return gioOT * cfg.donGiaTangCaMoiGio;
        }
    }
}
