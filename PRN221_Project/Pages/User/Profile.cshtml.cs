using Bogus.DataSets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.DataAccess.Repository.IRepository;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PRN221_Project.Pages.User
{
    public class ProfileModel : PageModel
    {
        private readonly IHostingEnvironment _environment;
        private readonly IUnitOfWork _unitOfWork;

        public ProfileModel(IUnitOfWork unitOfWork, IHostingEnvironment environment)
        {
            _environment = environment;
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public Project.Models.User UserAccount { get; set; } = default!;

        public IActionResult OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.User.GetFirstOrDefault(filter: u => u.Id == userId);
            if (user == null) return NotFound();
            UserAccount = user;
            return Page();
        }

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn 1 ảnh!")]
        [DataType(DataType.Upload)]
        [Display(Name = "Chọn 1 ảnh của bạn")]
        public IFormFile FileUpload { get; set; }
        private string[] permittedExtensions = { ".gif", ".png", ".jpg" };

        public async Task<IActionResult> OnPostAsync()
        {
            if (FileUpload == null)
            {
                return Page();
            }
            var ext = Path.GetExtension(FileUpload.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return Page();
            }
            Random random = new Random();
            string stringRandom = random.Next(1000000000).ToString();
            var file = Path.Combine(_environment.ContentRootPath + "wwwroot", "images", "Avatars", stringRandom + FileUpload.FileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
                await FileUpload.CopyToAsync(fileStream);
            UserAccount.UserImage = "/images/Avatars/" + stringRandom + FileUpload.FileName;

            UserAccount.Date_Edit = DateTime.Now;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //do model user ko co id nen ko the update
            //_unitOfWork.User.Update(UserAccount);
            //_unitOfWork.Save();

            return RedirectToPage("/Index");
        }
    }
}
