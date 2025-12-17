using ClinicManager.Models.Entities;

namespace ClinicManager.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Nếu đã có gói mặc định thì bỏ qua
            if (context.GoiDieuTris.Any(x => x.macDinh))
                return;

            var goiMacDinh = new GoiDieuTri
            {
                tenGoi = "Goi 1 buoi",
                tongSoBuoi = 1,
                donGia = 200000, // chỉnh theo phòng khám
                macDinh = true,
                hoatDong = true
            };

            context.GoiDieuTris.Add(goiMacDinh);
            context.SaveChanges();
        }
    }
}
