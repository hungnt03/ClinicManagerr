namespace ClinicManager.Models.Entities
{
    public class PhieuNhapKho
    {
        public int phieuNhapKhoId { get; set; }

        public DateTime ngayNhap { get; set; }
        public string? ghiChu { get; set; }

        public int nhanVienNhapId { get; set; }

        public decimal tongTien { get; set; }
        public string? duongDanHoaDon { get; set; }   // Bills/NhapKho/xxx.pdf

        public DateTime taoLuc { get; set; }
    }
}
