namespace ClinicManager.ViewModels.NhapKho
{
    public class NhapKhoCreateVm
    {
        public DateTime NgayNhap { get; set; } = DateTime.Today;
        public string? GhiChu { get; set; }

        // ===== HOA DON =====
        public IFormFile? HoaDonFile { get; set; }

        public List<NhapKhoItemVm> Items { get; set; } = new();
    }
}
