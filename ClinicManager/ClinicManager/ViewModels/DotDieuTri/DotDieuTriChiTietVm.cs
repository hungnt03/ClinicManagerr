namespace ClinicManager.ViewModels.DotDieuTri
{
    public class DotDieuTriChiTietVm
    {
        // ===== ID =====
        public int DotDieuTriId { get; set; }
        public int BenhNhanId { get; set; }

        // ===== KHÁM =====
        public DateTime NgayKham { get; set; }
        public string ChanDoan { get; set; }
        public string PhacDoDieuTri { get; set; }

        // ===== GÓI =====
        public int GoiDieuTriId { get; set; }
        public string TenGoi { get; set; }

        public int TongSoBuoi { get; set; }
        public int SoBuoiDaDung { get; set; }

        // ===== THANH TOÁN =====
        public decimal TongTien { get; set; }
        public decimal DaThanhToan { get; set; }

        // ===== TRẠNG THÁI =====
        public string TrangThai { get; set; }

        // ===== THỐNG KÊ NHANH =====
        public int SoBuoiConLai => TongSoBuoi - SoBuoiDaDung;
        public bool ConBuoi => SoBuoiDaDung < TongSoBuoi;

        // ===== THEO DÕI =====
        public DateTime TaoLuc { get; set; }
    }
}
