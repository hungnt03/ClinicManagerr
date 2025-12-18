using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.BenhNhan
{
    public class BenhNhanFormVm
    {
        public int BenhNhanId { get; set; }

        [Required]
        public string HoTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string GioiTinh { get; set; }

        [Required]
        public string SoDienThoai { get; set; }

        public string DiaChi { get; set; }

        public int? NguoiGioiThieuId { get; set; }

        // dropdown
        [ValidateNever]
        public List<NhanVienOptionVm> DanhSachNhanVien { get; set; }
    }
    public class NhanVienOptionVm
    {
        public int NhanVienId { get; set; }
        public string HoTen { get; set; }
    }
}
