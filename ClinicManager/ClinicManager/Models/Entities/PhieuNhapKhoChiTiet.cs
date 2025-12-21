namespace ClinicManager.Models.Entities
{
    public class PhieuNhapKhoChiTiet
    {
        public int phieuNhapKhoChiTietId { get; set; }

        public int phieuNhapKhoId { get; set; }
        public int vatTuId { get; set; }
        public VatTu vatTu { get; set; }

        public int soLuong { get; set; }
        public decimal donGiaNhap { get; set; }

        public decimal thanhTien { get; set; }
    }
}
