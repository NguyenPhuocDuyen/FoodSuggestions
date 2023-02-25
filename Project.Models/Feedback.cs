using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Feedback
    {
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public int FoodId { get; set; }
        [ForeignKey("FoodId")]
        public Food? Food { get; set; }
        [Required]
        public float Star { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
