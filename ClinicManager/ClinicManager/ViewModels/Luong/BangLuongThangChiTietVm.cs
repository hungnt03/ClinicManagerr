namespace ClinicManager.ViewModels.Luong
{
    public class BangLuongThangChiTietVm
    {
        public int BangLuongThangId { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public bool DaChot { get; set; }

        public List<BangLuongNhanVienVm> NhanViens { get; set; } = new();
    }
}
