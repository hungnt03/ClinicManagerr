namespace ClinicManager.Models.Entities
{
    public class BangLuongThang
    {
        public int bangLuongThangId { get; set; }


        public int thang { get; set; }   // 1–12
        public int nam { get; set; }

        public decimal tongLuong { get; set; }

        public bool daChot { get; set; }

        public DateTime taoLuc { get; set; }

        public ICollection<BangLuongThangChiTiet> ChiTiets { get; set; }
            = new List<BangLuongThangChiTiet>();
    }
}
