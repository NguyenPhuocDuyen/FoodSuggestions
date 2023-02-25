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
    public class Report
    {
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public int FoodId { get; set; }
        [ForeignKey("FoodId")]
        public Food? Food { get; set; }
        [Required]
        public string Reason { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int Status { get; set; }
    }
}
