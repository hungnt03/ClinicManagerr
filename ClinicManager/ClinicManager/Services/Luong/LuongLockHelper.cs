using ClinicManager.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services.Luong
{
    public static class LuongLockHelper
    {
        public static async Task<bool> DaChotLuongAsync(
        ApplicationDbContext context,
        DateTime ngay)
        {
            return await context.BangLuongThangs.AnyAsync(x =>
                x.thang == ngay.Month &&
                x.nam == ngay.Year &&
                x.daChot);
        }
    }
}
