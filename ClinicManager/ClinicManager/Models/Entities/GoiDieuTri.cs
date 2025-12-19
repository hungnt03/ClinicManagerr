namespace ClinicManager.Models.Entities
{
    public class GoiDieuTri
    {
        public int goiDieuTriId { get; set; }
        public string tenGoi { get; set; } // 1 buoi, 5 buoi, 10 buoi
        public int soBuoi { get; set; }
        public decimal gia { get; set; }
        public bool hoatDong { get; set; }
    }
}
