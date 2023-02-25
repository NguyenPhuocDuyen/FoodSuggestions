using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using System.ComponentModel.DataAnnotations;

using Project.Utility;

using System.Data;
using System.Security.Claims;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PRN221_Project.Pages.User.Restaurant
{
    [Authorize(Roles = SD.UserRole)]
    public class CreateModel : PageModel
    {
        private readonly IHostingEnvironment _environment;
        private readonly IUnitOfWork _unitOfWork;

        public CreateModel(IUnitOfWork unitOfWork, IHostingEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }


        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Project.Models.Restaurant Restaurant { get; set; } = default!;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn 1 ảnh!")]
        [DataType(DataType.Upload)]
        [Display(Name = "Chọn 1 ảnh mô tả nhà hàng")]
        public IFormFile FileUpload { get; set; }
        private string[] permittedExtensions = { ".gif", ".png", ".jpg", ".webp", ".jpeg"};

        /// <summary>
        /// add restaurant
        /// img and info restaurant
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (FileUpload != null)
            {
                var ext = Path.GetExtension(FileUpload.FileName).ToLowerInvariant();
                if (!string.IsNullOrEmpty(ext) && permittedExtensions.Contains(ext))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    //randow string to do not match the image
                    Random random = new Random();
                    string stringRandom = random.Next(1000000000).ToString() + userId;

                    var file = Path.Combine(_environment.ContentRootPath + "wwwroot", "images", "restaurants", stringRandom + FileUpload.FileName);
                    using (var fileStream = new FileStream(file, FileMode.Create))
                        await FileUpload.CopyToAsync(fileStream);

                    Restaurant.RestaurantImage = "/images/restaurants/" + stringRandom + FileUpload.FileName;
                    // set value default
                    Restaurant.UserId = userId;
                    _unitOfWork.Restaurant.Add(Restaurant);
                    _unitOfWork.Save();

                    return RedirectToPage("/User/Food/Create");
                }
            }
            ViewData["mess"] = ".gif .png .jpg .webp .jpeg";
            return Page();

        }
    }
}
