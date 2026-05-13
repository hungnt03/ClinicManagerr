namespace ClinicManager.Models.Entities
{
    public enum LoaiLuongChiTiet
    {
        LuongCoBan = 1,
        AnTrua = 2,
        XangXe = 3,
        ChuyenCan = 4,
        HoaHongDieuTriTay = 5,
        HoaHongTap = 6,
        HoaHongGioiThieu = 7,
        TangCa = 8,
        LuongNgayLe = 9,
        PhatSinh = 10
    }

    public class BangLuongThangChiTiet
    {
        public int bangLuongThangChiTietId { get; set; }

        public int bangLuongThangId { get; set; }
        public BangLuongThang BangLuongThang { get; set; }
        public int nhanVienId { get; set; }
        public NhanVien NhanVien { get; set; }

        public LoaiLuongChiTiet loai { get; set; }

        public decimal soTien { get; set; }

        public string dienGiai { get; set; }
    }
}
