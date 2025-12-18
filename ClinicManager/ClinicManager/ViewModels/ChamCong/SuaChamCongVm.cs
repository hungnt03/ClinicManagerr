using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.ChamCong
{
    public class SuaChamCongVm
    {
        public int ChamCongId { get; set; }

        [Required]
        public DateTime ThoiGianVao { get; set; }

        public DateTime? ThoiGianRa { get; set; }

        public bool NghiPhep { get; set; }
        public bool NghiPhepCoLuong { get; set; }

        [Required]
        public string LyDo { get; set; }
    }
}
