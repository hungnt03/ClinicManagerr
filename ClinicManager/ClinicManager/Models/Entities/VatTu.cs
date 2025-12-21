namespace ClinicManager.Models.Entities
{
    public class VatTu
    {
        public int vatTuId { get; set; }
        public string tenVatTu { get; set; }
        public LoaiVatTu loai { get; set; } // Thuoc, VatTu
        public string donViTinh { get; set; }
        public decimal donGia { get; set; }
        public int tonKho { get; set; }
    }
    public enum LoaiVatTu
    {
        Thuoc = 1,
        VatTu = 2
    }
}
