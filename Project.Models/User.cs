using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class User : IdentityUser
    {
        [Display(Name = "Ảnh đại diện")]
        public string? UserImage{ get; set; }
        [Required(ErrorMessage = "Vui lòng điền họ")]
        [Display(Name = "Họ")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Vui lòng điền tên")]
        [Display(Name = "Tên")]
        public string FirstName { get; set; }        
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; } = DateTime.Now;
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        public float? Latitute { get; set; }
        public float? Longitute { get; set; }
        public DateTime Date_Create { get; set; } = DateTime.Now;
        public DateTime? Date_Edit { get; set; }


        public ICollection<RoomChat>? Rooms { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
