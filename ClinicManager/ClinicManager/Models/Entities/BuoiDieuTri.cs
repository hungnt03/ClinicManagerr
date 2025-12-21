namespace ClinicManager.Models.Entities
{
    public class BuoiDieuTri
    {
        public int buoiDieuTriId { get; set; }

        // ===== LIÊN KẾT =====
        public int dotDieuTriId { get; set; }
        public DotDieuTri DotDieuTri { get; set; }
        public int benhNhanId { get; set; }
        public BenhNhan BenhNhan { get; set; }

        // ===== THỜI GIAN =====
        public DateTime ngayDieuTri { get; set; }

        // ===== NHÂN SỰ =====
        public int? bacSiDieuTriTayId { get; set; }
        public NhanVien BacSiDieuTriTay { get; set; }
        public int? kyThuatVienTapId { get; set; }
        public NhanVien NguoiTap { get; set; }

        // ===== NỘI DUNG =====
        public string? noiDungTap { get; set; }
        public string? noiDungDieuTriTay { get; set; }
        public string? chiDinhDacBiet { get; set; }

        // ===== CHI PHÍ PHÁT SINH =====
        public decimal chiPhiThuocVatTu { get; set; }

        // ===== THEO DÕI =====
        public DateTime taoLuc { get; set; }
    }
}
