using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.DotDieuTri
{
    public class DotDieuTriCreateVm
    {
        public int BenhNhanId { get; set; }
        public string TenBenhNhan { get; set; }

        [Required]
        public int BacSiKhamId { get; set; }

        [Required]
        public DateTime NgayKham { get; set; }

        public string TienSuBenh { get; set; }

        public string ChanDoan { get; set; }

        public string PhacDoDieuTri { get; set; }

        [Required]
        public int GoiDieuTriId { get; set; }

        public decimal PhanTramGiamGia { get; set; }

        // ===== DÙNG CHO VIEW =====
        [ValidateNever]
        public List<SelectListItem> BacSiList { get; set; } = new();
        [ValidateNever]
        public List<SelectListItem> GoiDieuTriList { get; set; } = new();
    }
}
