namespace ClinicManager.Models.Entities
{
    public class BuoiDieuTriNhanVien
    {
        public int id { get; set; }
        public int buoiDieuTriId { get; set; }
        public int nhanVienId { get; set; }
        public string vaiTro { get; set; } // Tap, DieuTriTay
    }
}
