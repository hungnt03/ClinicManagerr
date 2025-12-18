using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.NhanVien
{
    public class SuaNhanVienVm
    {
        public int NhanVienId { get; set; }

        [Required]
        public string HoTen { get; set; }

        [Required]
        public string VaiTroNhanVien { get; set; } // BacSi, KyThuatVien, LeTan

        [Required]
        [Range(0, double.MaxValue)]
        public decimal LuongCoBan { get; set; }

        public bool HoatDong { get; set; }

        // login
        public string EmailDangNhap { get; set; }
        public string RoleDangNhap { get; set; }

        // dropdown
        public string[] DanhSachVaiTroNhanVien { get; set; }
        public string[] DanhSachRoleDangNhap { get; set; }
    }
}
