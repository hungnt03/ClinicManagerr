using ClinicManager.Models.Entities;

namespace ClinicManager.ViewModels.Luong
{
    public class BangLuongNhanVienVm
    {
        public int nhanVienId { get; set; }
        public string TenNhanVien { get; set; }
        public decimal TongLuong { get; set; }

        public List<BangLuongThangChiTiet> ChiTiets { get; set; } = new();
    }
}
