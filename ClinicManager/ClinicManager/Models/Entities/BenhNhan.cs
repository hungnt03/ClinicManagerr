namespace ClinicManager.Models.Entities
{
    public class BenhNhan
    {
        public int benhNhanId { get; set; }
        public string hoTen { get; set; }
        public DateTime? ngaySinh { get; set; }
        public string gioiTinh { get; set; }
        public string soDienThoai { get; set; }
        public string diaChi { get; set; }
        // người giới thiệu (nhân viên)
        public int? nguoiGioiThieuId { get; set; }
        public DateTime taoLuc { get; set; }
    }
}
