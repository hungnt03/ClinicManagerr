using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.ThanhToan
{
    public class PhieuKhamVM
    {
        [Required]
        public int DotDieuTriId { get; set; }
        public DateTime NgayKham { get; set; }
        public string TenBenhNhan { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string TenGoiDangKy { get; set; }
        public string TienSuBenh { get; set; }
        public string TienLuong { get; set; }
        public string PhuongAn { get; set; }

    }
}
