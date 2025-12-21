using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.Luong
{
    public class CauHinhLuongVm
    {
        public int? CauHinhLuongId { get; set; }

        [Required]
        public TimeSpan GioBatDauSang { get; set; }

        [Required]
        public TimeSpan GioKetThucSang { get; set; }

        [Required]
        public TimeSpan GioBatDauChieu { get; set; }

        [Required]
        public TimeSpan GioKetThucChieu { get; set; }

        public decimal SoGioLamChuanNgay { get; set; }

        public decimal TienAnTruaNgay { get; set; }
        public decimal TienXangXeThang { get; set; }
        public decimal TienChuyenCan { get; set; }

        public decimal TienDieuTriTayMoiBuoi { get; set; }
        public decimal TienTapMoiBuoi { get; set; }
        public decimal PhanTramGioiThieu { get; set; }

        public decimal HeSoTangCaNgayThuong { get; set; }
        public decimal HeSoTangCaNgayLe { get; set; }

        public int SoPhutLamTronTangCa { get; set; }
        public int SoPhutToiThieuTinhTangCa { get; set; }

        [Required]
        public DateTime ApDungTuNgay { get; set; }
    }
}
