namespace ClinicManager.Models.Entities
{
    public class NhanVien
    {
        public int nhanVienId { get; set; }
        public string hoTen { get; set; }
        public string vaiTro { get; set; } // BacSi, KyThuatVien, LeTan
        public decimal luongCoBan { get; set; }
        public bool hoatDong { get; set; }
    }
}
