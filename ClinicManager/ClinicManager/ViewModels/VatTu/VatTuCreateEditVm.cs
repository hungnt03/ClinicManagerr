using ClinicManager.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.VatTu
{
    public class VatTuCreateEditVm
    {
        public int? VatTuId { get; set; }

        [Required]
        public string TenVatTu { get; set; }

        [Required]
        public LoaiVatTu Loai { get; set; }

        [Required]
        public string DonViTinh { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal DonGia { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int TonKho { get; set; }
    }
}
