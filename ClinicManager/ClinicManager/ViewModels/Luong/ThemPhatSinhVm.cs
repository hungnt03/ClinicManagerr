namespace ClinicManager.ViewModels.Luong
{
    public class ThemPhatSinhVm
    {
        public int BangLuongThangId { get; set; }
        public int NhanVienId { get; set; }
        public string TenNhanVien { get; set; }

        public decimal SoTien { get; set; }
        public int Loai { get; set; } // 1: Cộng, 2: Trừ
        public string NoiDung { get; set; }
    }
}
