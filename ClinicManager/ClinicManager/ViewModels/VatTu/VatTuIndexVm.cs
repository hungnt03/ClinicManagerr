namespace ClinicManager.ViewModels.VatTu
{
    public class VatTuIndexVm
    {
        public List<VatTuListVm> Items { get; set; } = new();

        public string? Keyword { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
