namespace ClinicManager.Models.Entities
{
    public class GoiDieuTri
    {
        public int goiDieuTriId { get; set; }
        public string tenGoi { get; set; }
        public int tongSoBuoi { get; set; }
        public decimal donGia { get; set; }
        public bool macDinh { get; set; }
        public bool hoatDong { get; set; }
    }
}
