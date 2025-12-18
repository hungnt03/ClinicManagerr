namespace ClinicManager.ViewModels.NhanVien
{
    public class NhanVienListItemVm
    {
        public int NhanVienId { get; set; }
        public string HoTen { get; set; }
        public string VaiTro { get; set; }
        public decimal LuongCoBan { get; set; }
        public bool HoatDong { get; set; } // nv còn làm hay đã nghỉ

        // thông tin tài khoản
        public string EmailDangNhap { get; set; }
        public bool CoTaiKhoan { get; set; }
        public bool BiKhoaTaiKhoan { get; set; }
    }
}
