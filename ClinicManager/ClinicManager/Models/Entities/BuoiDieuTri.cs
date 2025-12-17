namespace ClinicManager.Models.Entities
{
    public class BuoiDieuTri
    {
        public int buoiDieuTriId { get; set; }
        public int benhNhanId { get; set; }
        public int benhNhanGoiDieuTriId { get; set; }
        public DateTime ngayDieuTri { get; set; }
        public TimeSpan gioBatDau { get; set; }
        public TimeSpan gioKetThuc { get; set; }
        public string chiDinhDacBiet { get; set; }
        public string trangThai { get; set; }
    }
}
