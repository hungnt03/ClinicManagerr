using ClinicManager.Models.Entities;

namespace ClinicManager.ViewModels.ThanhToan
{
    public class PhieuThuVm
    {
        public int ThanhToanId { get; set; }

        public string SoPhieu { get; set; } = string.Empty;

        public DateTime NgayThu { get; set; }

        public string TenBenhNhan { get; set; } = string.Empty;

        public string NoiDungThu { get; set; } = string.Empty;

        public decimal SoTien { get; set; }

        public HinhThucThanhToan HinhThuc { get; set; }

        public string? GhiChu { get; set; }
    }
}
