using System.ComponentModel.DataAnnotations;
using ClinicManager.Models.Entities;

namespace ClinicManager.ViewModels.ThanhToan
{
    public class ThuTienGoiVm
    {
        [Required]
        public int DotDieuTriId { get; set; }

        public decimal TongTien { get; set; }

        public decimal PhanTramGiamGia { get; set; }

        public decimal TienGiam =>
            Math.Round(TongTien * PhanTramGiamGia / 100, 0);

        public decimal TongTienSauGiam =>
            TongTien - TienGiam;

        public decimal DaThanhToan { get; set; }

        public decimal ConLai =>
            TongTienSauGiam - DaThanhToan;

        [Required]
        [Range(1, double.MaxValue)]
        public decimal SoTienThu { get; set; }

        [Required]
        public HinhThucThanhToan HinhThuc { get; set; }

        public string? GhiChu { get; set; }
    }

}
