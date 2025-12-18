namespace ClinicManager.ViewModels.ChamCong
{
    public class ChamCongChiTietVm
    {
        public int ChamCongId { get; set; }
        public DateTime Ngay { get; set; }

        public DateTime GioVao { get; set; }
        public DateTime? GioRa { get; set; }

        public bool NghiPhep { get; set; }
        public bool NghiPhepCoLuong { get; set; }
        public bool AnTrua { get; set; }
    }
}
