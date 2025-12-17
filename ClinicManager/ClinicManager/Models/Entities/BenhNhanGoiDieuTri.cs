namespace ClinicManager.Models.Entities
{
    public class BenhNhanGoiDieuTri
    {
        public int benhNhanGoiDieuTriId { get; set; }
        public int benhNhanId { get; set; }
        public int khamBenhId { get; set; }
        public int goiDieuTriId { get; set; }
        public int tongSoBuoi { get; set; }
        public int soBuoiDaDung { get; set; }
        public DateTime ngayMua { get; set; }
        public string trangThai { get; set; }
    }
}
