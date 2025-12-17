namespace ClinicManager.Models.Entities
{
    public class DatLichKham
    {
        public int datLichKhamId { get; set; }
        public int? benhNhanId { get; set; }
        public string hoTen { get; set; }
        public string soDienThoai { get; set; }
        public DateTime thoiGianHen { get; set; }
        public int? bacSiDuKienId { get; set; }
        public string trangThai { get; set; } // DatHen, DaDen, Huy
        public string ghiChu { get; set; }
        public DateTime taoLuc { get; set; }
    }
}
