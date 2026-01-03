namespace ClinicManager.ViewModels.Dashboard
{
    public class DashboardBacSiVm
    {
        // ===== HÔM NAY =====
        public int SoBenhNhanHomNay { get; set; }
        public int SoBuoiDieuTriHomNay { get; set; }

        // ===== CÔNG VIỆC =====
        public List<BuoiDieuTriHomNayVm> BuoiHomNay { get; set; } = new();
    }
    public class BuoiDieuTriHomNayVm
    {
        public int BuoiDieuTriId { get; set; }
        public int DotDieuTriId { get; set; }

        public string TenBenhNhan { get; set; }
        public DateTime NgayDieuTri { get; set; }

        public bool DieuTriTay { get; set; }
        public bool Tap { get; set; }

        public string TrangThai { get; set; }
    }
}
