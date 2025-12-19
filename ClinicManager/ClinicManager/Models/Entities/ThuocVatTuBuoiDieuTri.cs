namespace ClinicManager.Models.Entities
{
    public class ThuocVatTuBuoiDieuTri
    {
        public int id { get; set; }
        public int buoiDieuTriId { get; set; }
        public BuoiDieuTri BuoiDieuTri { get; set; }
        public int vatTuId { get; set; }
        public VatTu VatTu { get; set; }
        public int soLuong { get; set; }
        public decimal donGia { get; set; }
    }
}
