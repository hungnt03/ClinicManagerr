using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManager.ViewModels.Luong
{
    public class NgayLeVm
    {
        public int? NgayLeId { get; set; }

        [Required]
        public DateTime Ngay { get; set; }

        [Required]
        public string Ten { get; set; }

        public bool CoTinhLuong { get; set; }
    }
}
