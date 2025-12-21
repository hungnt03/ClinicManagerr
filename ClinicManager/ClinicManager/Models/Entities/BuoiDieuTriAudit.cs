namespace ClinicManager.Models.Entities
{
    public class BuoiDieuTriAudit
    {
        public int buoiDieuTriAuditId { get; set; }

        public int buoiDieuTriId { get; set; }
        public int adminNhanVienId { get; set; }

        // ===== TRƯỚC KHI SỬA =====
        public DateTime ngayDieuTriCu { get; set; }
        public int? bacSiDieuTriTayIdCu { get; set; }
        public int? kyThuatVienTapIdCu { get; set; }
        public string? noiDungTapCu { get; set; }
        public string? noiDungDieuTriTayCu { get; set; }
        public string? chiDinhDacBietCu { get; set; }
        public decimal chiPhiThuocVatTuCu { get; set; }

        // ===== SAU KHI SỬA =====
        public DateTime ngayDieuTriMoi { get; set; }
        public int? bacSiDieuTriTayIdMoi { get; set; }
        public int? kyThuatVienTapIdMoi { get; set; }
        public string? noiDungTapMoi { get; set; }
        public string? noiDungDieuTriTayMoi { get; set; }
        public string? chiDinhDacBietMoi { get; set; }
        public decimal chiPhiThuocVatTuMoi { get; set; }

        public string lyDo { get; set; }
        public DateTime suaLuc { get; set; }
    }

}
