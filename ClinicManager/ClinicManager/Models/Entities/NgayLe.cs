using System.Net.NetworkInformation;

namespace ClinicManager.Models.Entities
{
    /// <summary>
    /// Ngày lễ đặc biệt trong năm
    /// Có thể dùng cho:
    //    Tết
    //    Quốc khánh
    //    Lễ nội bộ phòng khám
    //Chủ nhật KHÔNG cần lưu vào đây
    /// </summary>
    public class NgayLe
    {
        public int ngayLeId { get; set; }

        public DateTime ngay { get; set; }

        public string ten { get; set; }

        /// <summary>
        /// true: nghỉ nhưng vẫn tính lương
        /// false: nghỉ không lương
        /// </summary>
        public bool coTinhLuong { get; set; }

        public DateTime taoLuc { get; set; }
    }
}
