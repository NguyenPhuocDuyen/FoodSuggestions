using System.ComponentModel.DataAnnotations;

namespace PRN221_Project.ViewModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
