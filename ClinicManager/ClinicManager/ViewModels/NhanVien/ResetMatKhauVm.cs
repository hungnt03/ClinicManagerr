using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.NhanVien
{
    public class ResetMatKhauVm
    {
        public int NhanVienId { get; set; }

        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string MatKhauMoi { get; set; }

        [Required]
        [Compare("MatKhauMoi", ErrorMessage = "Mat khau xac nhan khong khop")]
        public string XacNhanMatKhau { get; set; }
    }
}
