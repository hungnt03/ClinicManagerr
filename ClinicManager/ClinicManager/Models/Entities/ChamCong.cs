namespace ClinicManager.Models.Entities
{
    public class ChamCong
    {
        public int chamCongId { get; set; }
        public int nhanVienId { get; set; }
        public DateTime thoiGianVao { get; set; }
        public DateTime? thoiGianRa { get; set; }
        public bool nghiPhep { get; set; }
        public bool nghiPhepCoLuong { get; set; }
        public bool anTrua { get; set; }
    }
}
