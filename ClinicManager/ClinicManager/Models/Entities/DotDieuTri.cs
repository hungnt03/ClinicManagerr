namespace ClinicManager.Models.Entities
{
    /// <summary>
    /// soBuoiDaDung <= tongSoBuoi
    //Khi soBuoiDaDung == tongSoBuoi → HoanThanh
    //KHÔNG xóa DotDieuTri(chỉ đổi trạng thái)
    /// </summary>
    public class DotDieuTri
    {
        public int dotDieuTriId { get; set; }

        // ===== LIÊN KẾT =====
        public int benhNhanId { get; set; }
        public  BenhNhan BenhNhan { get; set; }
        public int bacSiKhamId { get; set; }

        // ===== KHÁM =====
        public DateTime ngayKham { get; set; }
        public string chanDoan { get; set; }
        public string phacDoDieuTri { get; set; }

        // ===== GÓI =====
        public int goiDieuTriId { get; set; }   // 1, 5, 10 buổi
        public GoiDieuTri GoiDieuTri { get; set; }
        public int tongSoBuoi { get; set; }
        public int soBuoiDaDung { get; set; }

        // ===== THANH TOÁN =====
        public decimal tongTien { get; set; }
        public decimal daThanhToan { get; set; }

        // ===== TRẠNG THÁI =====
        public TrangThaiDotDieuTri trangThai { get; set; }

        public decimal phanTramGiamGia { get; set; } // 0–100

        public DateTime? ngayThanhToan { get; set; }

        // ===== THEO DÕI =====
        public DateTime taoLuc { get; set; }

        public ICollection<BuoiDieuTri> BuoiDieuTris { get; set; }
            = new List<BuoiDieuTri>();
    }

    public enum TrangThaiDotDieuTri
    {
        MoiTao = 0,
        DangDieuTri = 1,
        TamDung = 2,
        HoanThanh = 3
    }
}
