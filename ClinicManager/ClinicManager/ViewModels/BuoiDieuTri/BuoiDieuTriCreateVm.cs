using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.BuoiDieuTri
{
    public class BuoiDieuTriCreateVm
    {
        [Required]
        public int DotDieuTriId { get; set; }

        [Required]
        public int BenhNhanId { get; set; }

        [Required]
        public DateTime NgayDieuTri { get; set; }

        public int? BacSiDieuTriTayId { get; set; }
        public int? KyThuatVienTapId { get; set; }

        public string? NoiDungTap { get; set; }
        public string? NoiDungDieuTriTay { get; set; }
        public string? ChiDinhDacBiet { get; set; }

        // ===== DÙNG CHO VIEW =====
        [ValidateNever]
        public List<SelectListItem> DanhSachBacSi { get; set; } = new();
        [ValidateNever]
        public List<SelectListItem> DanhSachKyThuatVien { get; set; } = new();

        // ===== CHỈ SET SAU KHI TẠO BUỔI =====
        public int? BuoiDieuTriId { get; set; }
    }
}
