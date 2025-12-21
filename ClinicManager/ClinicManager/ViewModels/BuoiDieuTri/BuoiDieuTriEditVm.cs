using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.BuoiDieuTri
{
    public class BuoiDieuTriEditVm
    {
        // ===== KEY =====
        [Required]
        public int BuoiDieuTriId { get; set; }

        [Required]
        public int DotDieuTriId { get; set; }

        [Required]
        public int BenhNhanId { get; set; }

        // ===== THỜI GIAN =====
        [Required]
        public DateTime NgayDieuTri { get; set; }

        // ===== NHÂN SỰ =====
        public int? BacSiDieuTriTayId { get; set; }
        public int? KyThuatVienTapId { get; set; }

        // ===== NỘI DUNG =====
        public string? NoiDungTap { get; set; }
        public string? NoiDungDieuTriTay { get; set; }
        public string? ChiDinhDacBiet { get; set; }

        // ===== CHI PHÍ =====
        public decimal ChiPhiThuocVatTu { get; set; }

        // ===== AUDIT =====
        [Required(ErrorMessage = "Bat buoc nhap ly do sua")]
        public string LyDoSua { get; set; }

        // ===== DÙNG CHO VIEW =====
        [ValidateNever]
        public List<SelectListItem> DanhSachBacSiDieuTriTay { get; set; } = new();

        [ValidateNever]
        public List<SelectListItem> DanhSachNguoiTap { get; set; } = new();
    }
}
