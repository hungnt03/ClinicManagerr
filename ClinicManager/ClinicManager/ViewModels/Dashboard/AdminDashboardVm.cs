namespace ClinicManager.ViewModels.Dashboard
{
    public class AdminDashboardVm
    {
        // ===== KPI =====
        public decimal DoanhThuThang { get; set; }
        public decimal ChiPhiLuongThang { get; set; }
        public int SoBenhNhanThang { get; set; }
        public int SoBuoiDieuTriThang { get; set; }

        // ===== CHART =====
        public List<string> Labels { get; set; } = new();
        public List<decimal> DoanhThuSeries { get; set; } = new();
        public List<decimal> ChiPhiLuongSeries { get; set; } = new();
        public List<int> BenhNhanSeries { get; set; } = new();
    }
}

