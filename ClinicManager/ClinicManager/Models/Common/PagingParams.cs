namespace ClinicManager.Models.Common
{
    public class PagingParams
    {
        public string SearchString { get; set; }
        public int? Page { get; set; }
        public int PageSize { get; set; } = 10; // Mặc định 10 dòng/trang
    }
}
