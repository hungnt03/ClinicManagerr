namespace ClinicManager.ViewModels.ChamCong
{
    public class ChamCongHomNayVm
    {
        public bool DaCheckIn { get; set; }
        public bool DaCheckOut { get; set; }

        public DateTime? GioVao { get; set; }
        public DateTime? GioRa { get; set; }

        public bool AnTrua { get; set; }
        public bool NghiPhep { get; set; }
    }
}
