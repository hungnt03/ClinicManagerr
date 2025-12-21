using System.ComponentModel.DataAnnotations;
using ClinicManager.Models.Entities;

namespace ClinicManager.ViewModels.ThanhToan
{
    public class ThuTienGoiVm
    {
        [Required]
        public int DotDieuTriId { get; set; }

        public decimal TongTien { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal ConLai => TongTien - DaThanhToan;

        [Required]
        [Range(1, double.MaxValue)]
        public decimal SoTienThu { get; set; }

        [Required]
        public HinhThucThanhToan HinhThuc { get; set; }

        public string? GhiChu { get; set; }
    }
}
