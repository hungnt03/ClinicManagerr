namespace ClinicManager.ViewModels.Luong
{
    public class BangLuongThangListItemVm
    {
        public int BangLuongThangId { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }

        public int SoNhanVien { get; set; }
        public decimal TongLuong { get; set; }

        public bool DaChot { get; set; }
        public DateTime TaoLuc { get; set; }
    }
}
