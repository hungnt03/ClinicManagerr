namespace ClinicManager.Services
{
    using ClinicManager.Data;
    using ClinicManager.Models.Entities;
    using ClinicManager.ViewModels.VatTu;
    using Microsoft.EntityFrameworkCore;

    public interface IVatTuService
    {
        Task<List<VatTuListVm>> GetAllAsync();
        Task<VatTuCreateEditVm?> GetByIdAsync(int id);
        Task CreateAsync(VatTuCreateEditVm vm);
        Task UpdateAsync(VatTuCreateEditVm vm);
        Task<VatTuIndexVm> SearchAsync(
        string? keyword,
        int page,
        int pageSize);
    }

    public class VatTuService : IVatTuService
    {
        private readonly ApplicationDbContext _context;

        public VatTuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VatTuListVm>> GetAllAsync()
        {
            return await _context.VatTus
                .OrderBy(x => x.tenVatTu)
                .Select(x => new VatTuListVm
                {
                    VatTuId = x.vatTuId,
                    TenVatTu = x.tenVatTu,
                    Loai = x.loai,
                    DonViTinh = x.donViTinh,
                    DonGia = x.donGia,
                    TonKho = x.tonKho
                })
                .ToListAsync();
        }

        public async Task<VatTuCreateEditVm?> GetByIdAsync(int id)
        {
            return await _context.VatTus
                .Where(x => x.vatTuId == id)
                .Select(x => new VatTuCreateEditVm
                {
                    VatTuId = x.vatTuId,
                    TenVatTu = x.tenVatTu,
                    Loai = x.loai,
                    DonViTinh = x.donViTinh,
                    DonGia = x.donGia,
                    TonKho = x.tonKho
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(VatTuCreateEditVm vm)
        {
            var entity = new VatTu
            {
                tenVatTu = vm.TenVatTu,
                loai = vm.Loai,
                donViTinh = vm.DonViTinh,
                donGia = vm.DonGia,
                tonKho = vm.TonKho
            };

            _context.VatTus.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(VatTuCreateEditVm vm)
        {
            var entity = await _context.VatTus
                .FirstOrDefaultAsync(x => x.vatTuId == vm.VatTuId);

            if (entity == null)
                throw new Exception("Vat tu khong ton tai");

            entity.tenVatTu = vm.TenVatTu;
            entity.loai = vm.Loai;
            entity.donViTinh = vm.DonViTinh;
            entity.donGia = vm.DonGia;
            entity.tonKho = vm.TonKho;

            await _context.SaveChangesAsync();
        }
        public async Task<VatTuIndexVm> SearchAsync(
    string? keyword,
    int page,
    int pageSize)
        {
            var query = _context.VatTus.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.tenVatTu.Contains(keyword));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.tenVatTu)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new VatTuListVm
                {
                    VatTuId = x.vatTuId,
                    TenVatTu = x.tenVatTu,
                    Loai = x.loai,
                    DonViTinh = x.donViTinh,
                    DonGia = x.donGia,
                    TonKho = x.tonKho
                })
                .ToListAsync();

            return new VatTuIndexVm
            {
                Items = items,
                Keyword = keyword,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }

    }
}