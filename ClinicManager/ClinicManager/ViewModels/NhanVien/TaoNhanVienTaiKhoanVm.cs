using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.NhanVien
{
    public class TaoNhanVienTaiKhoanVm
    {
        [Required]
        public string HoTen { get; set; }

        [Required]
        public string VaiTroNhanVien { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal LuongCoBan { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string MatKhau { get; set; }

        [Required]
        public string RoleDangNhap { get; set; }

        // dùng cho dropdown
        [ValidateNever]
        public string[] DanhSachVaiTro { get; set; }
        [ValidateNever]
        public string[] DanhSachRole { get; set; }
    }
}
