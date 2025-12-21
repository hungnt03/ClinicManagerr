using ClinicManager.Models.Entities;

namespace ClinicManager.ViewModels.VatTu
{
    public class VatTuListVm
    {
        public int VatTuId { get; set; }
        public string TenVatTu { get; set; }

        public LoaiVatTu Loai { get; set; }

        public string DonViTinh { get; set; }
        public decimal DonGia { get; set; }
        public int TonKho { get; set; }

        public string TenLoai =>
            Loai == LoaiVatTu.Thuoc ? "Thuốc" : "Vật tư";
    }
}
