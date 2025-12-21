using ClinicManager.Models.Entities;

namespace ClinicManager.Services
{
    public class LuongTangCaHelper
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
            if (tongPhut < 60)
                return 0;

            return Math.Floor(
                (decimal)tongPhut / buocLamTron
            ) * (buocLamTron / 60m);
        }

        public static decimal TinhOTChoNgay(
            ChamCong cc,
            CauHinhLuong cfg,
            decimal luongMoiGio,
            bool laNgayDacBiet)
        {
            if (!cc.thoiGianRa.HasValue)
                return 0;

            int phutOT = 0;

            // === ĐẾN SỚM ===
            var gioBatDauChuan =
                cc.thoiGianVao.Date + cfg.gioBatDauSang;

            if (cc.thoiGianVao < gioBatDauChuan)
            {
                phutOT += (int)(gioBatDauChuan - cc.thoiGianVao).TotalMinutes;
            }

            // === VỀ MUỘN ===
            var gioKetThucChuan =
                cc.thoiGianVao.Date + cfg.gioKetThucChieu;

            if (cc.thoiGianRa.Value > gioKetThucChuan)
            {
                phutOT += (int)(cc.thoiGianRa.Value - gioKetThucChuan).TotalMinutes;
            }

            var gioOT = LamTronGioOT(
                phutOT,
                cfg.soPhutLamTronTangCa);

            if (gioOT <= 0)
                return 0;

            var heSo = laNgayDacBiet
                ? cfg.heSoTangCaNgayLe
                : cfg.heSoTangCaNgayThuong;

            return gioOT * luongMoiGio * heSo;
        }
    }
}
