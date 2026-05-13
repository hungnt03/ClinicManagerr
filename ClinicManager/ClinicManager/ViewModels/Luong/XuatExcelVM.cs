using ClinicManager.Models.Entities;

namespace ClinicManager.ViewModels.Luong
{
    public class XuatExcelVM
    {
        public int BangLuongThangId { get; set; }
        public DateTime NgayThang { get; set; }
        public List<ThuExcelVM> ThuList { get; set; }
        public List<ChiExcelVM> ChiList { get; set; }
        public List<ChamCongExcelVM> ChamCongList { get; set; }
        public List<TheoDoiDieuTriExcelVM> TheoDoiDieuTriList { get; set; }
        public List<LuongExcelVM> LuongList { get; set; }
        public CauHinhLuong cfg { get; set; }
    }

    public class ThuExcelVM
    {
        public int DotDieuTriId { get; set; }
        public DateTime NgayThang { get; set; }
        public string HoTen { get; set; }
        public string GoiDieuTri { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string GhiChu { get; set; }
        public decimal TienThuoc { get; set; }
        public string TenThuoc { get; set; }
    }
    public class ChiExcelVM
    {
        public DateTime NgayThang { get; set; }
        public string TenKhoanChi { get; set; }
        public decimal ThanhTien { get; set; }
        public string GhiChu { get; set; }
    }
    public class  ChamCongExcelVM
    {
        public int NhanVienId { get; set; }
        public string HoTen { get; set; }
        public DateTime? Checkin { get; set; }
        public DateTime? Checkout { get; set; }
        public bool DiLam { get; set; }
        public decimal? TangCa { get; set; } = 0;
        public decimal? TienTangCa { get; set; } = 0;
        public bool ComTrua { get; set; }
    }
    public class TheoDoiDieuTriExcelVM
    {
        public int No { get; set; }
        public int BenhNhanId { get; set; }
        public string HoTenBenhNhan { get; set; }
        public DateTime NgayThang { get; set; }
        public bool DieuTriTay { get; set; }
        public int? BsDieuTriTayId { get; set; }
        public string? TenBsDieuTriTay { get; set; }
        public bool DieuTriTap { get; set; }
        public int? BsDieuTriTapId { get; set; }
        public string? TenBsDieuTriTap { get; set; }
        public bool HoanThanh { get; set; }

    }
    public class LuongExcelVM
    {
        public int NhanVienId { get; set; }
        public string HoTen { get; set; }
        public List<LuongChiTietExcelVM> LuongChiTietList { get; set; }
    }
    public class LuongChiTietExcelVM
    {
        public LoaiLuongChiTiet Loai { get; set; }
        public decimal SoTien { get; set; } = 0;
        public string GhiChu { get; set; }
    }
}
