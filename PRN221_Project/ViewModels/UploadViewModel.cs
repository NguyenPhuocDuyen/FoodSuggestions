using System.ComponentModel.DataAnnotations;

namespace PRN221_Project.ViewModels
{
    public class UploadViewModel
    {
        [Required]
        public int RoomId { get; set; }
        public IFormFile? File { get; set; }
    }
}
