namespace ClinicManager.Models.Entities
{
    public class DotDieuTriMuaThem
    {
        public int id { get; set; }
        public int dotDieuTriId { get; set; }

        public int soBuoiThem { get; set; }
        public decimal soTien { get; set; }

        public DateTime muaLuc { get; set; }
    }

}
