using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Restaurant
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên nhà hàng không được để trống")]
        public string Name { get; set; }
        public string? RestaurantImage { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        [Phone]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Mô tả không được để trống")]
        public string Description { get; set; }
        public DateTime Date_Create { get; set; } = DateTime.Now;
        public DateTime? Date_Edit { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

    }
}
