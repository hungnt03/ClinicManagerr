namespace ClinicManager.ViewModels.BenhNhan
{
    public class BenhNhanChiTietVm
    {
        public int BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }

        public List<DotDieuTriItemVm> DotDieuTris { get; set; } = new();
    }

    public class DotDieuTriItemVm
    {
        public int DotDieuTriId { get; set; }
        public DateTime NgayKham { get; set; }
        public string ChanDoan { get; set; }
        public string PhacDoDieuTri { get; set; }

        public int TongSoBuoi { get; set; }
        public int SoBuoiDaDung { get; set; }

        public decimal TongTien { get; set; }
        public decimal DaThanhToan { get; set; }

        public string TrangThai { get; set; }
    }
}
