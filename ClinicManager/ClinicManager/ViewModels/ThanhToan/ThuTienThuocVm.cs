using System.ComponentModel.DataAnnotations;
using ClinicManager.Models.Entities;

namespace ClinicManager.ViewModels.ThanhToan
{
    public class ThuTienThuocVm
    {
        [Required]
        public int BuoiDieuTriId { get; set; }

        public decimal TongChiPhi { get; set; }

        public decimal DaThu { get; set; }

        public decimal ConLai => TongChiPhi - DaThu;

        [Required]
        [Range(1, double.MaxValue)]
        public decimal SoTienThu { get; set; }

        [Required]
        public HinhThucThanhToan HinhThuc { get; set; }

        public string? GhiChu { get; set; }
    }
}
