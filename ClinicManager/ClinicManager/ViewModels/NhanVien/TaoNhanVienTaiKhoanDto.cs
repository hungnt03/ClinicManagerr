namespace ClinicManager.ViewModels.NhanVien
{
    public class TaoNhanVienTaiKhoanDto
    {
        // Thông tin nhân viên
        public string hoTen { get; set; }
        public string vaiTro { get; set; } // BacSi, KyThuatVien, LeTan
        public decimal luongCoBan { get; set; }

        // Thông tin tài khoản
        public string email { get; set; }
        public string matKhau { get; set; }
        public string role { get; set; } // Admin, BacSi, KyThuatVien, LeTan
    }

}
