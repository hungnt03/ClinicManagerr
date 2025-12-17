namespace ClinicManager.Models.Entities
{
    public class KhamBenh
    {
        public int khamBenhId { get; set; }
        public int benhNhanId { get; set; }
        public int bacSiId { get; set; }
        public DateTime ngayKham { get; set; }
        public string chanDoan { get; set; }
        public string phacDoDieuTri { get; set; }
        public int goiDieuTriId { get; set; }
        public string tinhTrangThanhToan { get; set; }
        public DateTime taoLuc { get; set; }
    }
}
