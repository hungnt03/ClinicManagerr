namespace ClinicManager.ViewModels.DotDieuTri
{
    public class BuoiDieuTriItemVm
    {
        public int BuoiDieuTriId { get; set; }
        public DateTime NgayDieuTri { get; set; }

        public string? NoiDungTap { get; set; }
        public string? NoiDungDieuTriTay { get; set; }
        public string? ChiDinhDacBiet { get; set; }

        public string? TenBacSiDieuTriTay { get; set; }
        public string? TenNguoiTap { get; set; }

        public decimal ChiPhiThuocVatTu { get; set; }
    }

}
