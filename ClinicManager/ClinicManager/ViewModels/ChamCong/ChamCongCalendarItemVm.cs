namespace ClinicManager.ViewModels.ChamCong
{
    public class ChamCongCalendarItemVm
    {
        public int ChamCongId { get; set; }
        public DateTime Ngay { get; set; }

        public DateTime? GioVao { get; set; }
        public DateTime? GioRa { get; set; }

        public bool NghiPhep { get; set; }
        public bool DiMuon { get; set; }
        public bool VeSom { get; set; }
        public bool LaCuoiTuan { get; set; }

        public double? SoGioLam { get; set; }

        public string BackgroundColor { get; set; }
    }
}
