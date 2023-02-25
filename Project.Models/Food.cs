using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Food
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên món ăn không được để trống")]
        public string Name { get; set; }
        [Required]
        public string FoodImage { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public int RestaurantId { get; set; }
        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }
        [Required(ErrorMessage = "Mô tả món ăn không được bỏ trống")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Giá món ăn không được để trống")]
        [Range(1000, int.MaxValue, ErrorMessage = "Giá món ăn lớn hơn 1000")]
        public int Price { get; set; }
        public DateTime Date_Create { get; set; } = DateTime.Now;
        public DateTime? Date_Edit { get; set; } = DateTime.Now;
        public bool? IsBlackList { get; set; } = false;
        public string? Reason { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
