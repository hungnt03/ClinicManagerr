namespace ClinicManager.Models.Entities
{
    public class ChamCongAudit
    {
        public int chamCongAuditId { get; set; }
        public int chamCongId { get; set; }
        public int adminNhanVienId { get; set; }

        public DateTime? thoiGianVaoCu { get; set; }
        public DateTime? thoiGianRaCu { get; set; }
        public bool nghiPhepCu { get; set; }
        public bool nghiPhepCoLuongCu { get; set; }

        public DateTime? thoiGianVaoMoi { get; set; }
        public DateTime? thoiGianRaMoi { get; set; }
        public bool nghiPhepMoi { get; set; }
        public bool nghiPhepCoLuongMoi { get; set; }

        public string lyDo { get; set; }
        public DateTime suaLuc { get; set; }
    }
}
