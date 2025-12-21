using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.NhapKho
{
    public class NhapKhoItemVm
    {
        [Required]
        public int VatTuId { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [Required]
        public decimal DonGiaNhap { get; set; }
    }
}
