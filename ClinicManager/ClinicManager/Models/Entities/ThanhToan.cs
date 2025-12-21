namespace ClinicManager.Models.Entities
{
    public class ThanhToan
    {
        public int thanhToanId { get; set; }

        public LoaiThanhToan loai { get; set; }

        // ===== LIÊN KẾT =====
        public int? dotDieuTriId { get; set; }
        public int? buoiDieuTriId { get; set; }

        // ===== SỐ TIỀN =====
        public decimal soTien { get; set; }

        // ===== THÔNG TIN THU =====
        public DateTime ngayThu { get; set; }
        public HinhThucThanhToan hinhThuc { get; set; }

        public string? ghiChu { get; set; }

        // ===== THEO DÕI =====
        public bool daChot { get; set; }   // khóa khi chốt tháng
        public DateTime taoLuc { get; set; }
    }

    public enum LoaiThanhToan
    {
        GoiDieuTri = 1,
        ThuocVatTu = 2
    }
    public enum HinhThucThanhToan
    {
        TienMat = 1,
        ChuyenKhoan = 2,
        QuetQR = 3
    }

}
