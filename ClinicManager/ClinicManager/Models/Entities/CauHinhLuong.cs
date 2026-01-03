namespace ClinicManager.Models.Entities
{
    /// <summary>
    /// Cấu hình lương – KHÔNG hard-code
    /// Có hiệu lực từ ngày apDungTuNgay
    /// </summary>
    public class CauHinhLuong
    {
        public int cauHinhLuongId { get; set; }

        // ===== GIỜ LÀM CHUẨN =====
        public TimeSpan gioBatDauSang { get; set; }     // 08:00
        public TimeSpan gioKetThucSang { get; set; }    // 12:00
        public TimeSpan gioBatDauChieu { get; set; }    // 13:30
        public TimeSpan gioKetThucChieu { get; set; }   // 17:30

        public decimal soGioLamChuanNgay { get; set; }  // 8

        // ===== PHỤ CẤP =====
        public decimal tienAnTruaNgay { get; set; }     // 30000
        public decimal tienXangXeThang { get; set; }    // 500000
        public decimal tienChuyenCan { get; set; }      // 200000

        // ===== HOA HỒNG =====
        public decimal tienDieuTriTayMoiBuoi { get; set; } // 30000
        public decimal tienTapMoiBuoi { get; set; }       // 20000
        public decimal phanTramGioiThieu { get; set; }    // 5 (%)

        // ===== TĂNG CA =====
        //public decimal heSoTangCaNgayThuong { get; set; } // 1.25
        //public decimal heSoTangCaNgayLe { get; set; }     // 1.50

        public int soPhutLamTronTangCa { get; set; }      // 30 phút
        public int soPhutToiThieuTinhTangCa { get; set; } // 60 phút
        public decimal donGiaTangCaMoiGio { get; set; }     // 50000

        // ===== HIỆU LỰC =====
        public DateTime apDungTuNgay { get; set; }

        public DateTime taoLuc { get; set; }
    }
}
