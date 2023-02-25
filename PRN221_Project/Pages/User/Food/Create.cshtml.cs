using Bogus.DataSets;
using Bogus.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PRN221_Project.Pages.User.Food
{
    [BindProperties]    
    
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostingEnvironment _environment;

        public CreateModel(IUnitOfWork unitOfWork, IHostingEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }


        public IActionResult OnGet()
        {
            ViewData["Restaurants"] = new SelectList(_unitOfWork.Restaurant.GetAll().OrderByDescending(r => r.Date_Create), "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Project.Models.Food Food { get; set; } = default!;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn 1 ảnh!")]
        [DataType(DataType.Upload)]
        [Display(Name = "Chọn 1 ảnh mô tả món ăn")]
        public IFormFile FileUpload { get; set; }
        private string[] permittedExtensions = { ".gif", ".png", ".jpg", ".webp", ".jpeg"};

        /// <summary>
        /// add new food from user
        /// add img and info food
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
                    string stringRandom = random.Next(1000000000).ToString()+userId;
                    var file = Path.Combine(_environment.ContentRootPath + "wwwroot", "images", "foods", stringRandom + FileUpload.FileName);
                    using (var fileStream = new FileStream(file, FileMode.Create))
                        await FileUpload.CopyToAsync(fileStream);

                    Food.FoodImage = "/images/foods/" + stringRandom + FileUpload.FileName;
                    //set default info food
                    Food.UserId = userId;
                    _unitOfWork.Food.Add(Food);
                    _unitOfWork.Save();

                    // food added by user will be user's favorite food
                    var foodCurrent = _unitOfWork.Food
                        .GetAll(filter: f => f.IsDeleted == false)
                        .OrderByDescending(f => f.Id).FirstOrDefault();
                    FoodOfUser foodOfUser = new FoodOfUser();
                    foodOfUser.UserId = userId;
                    foodOfUser.FoodId = foodCurrent.Id;
                    _unitOfWork.FoodOfUser.Add(foodOfUser);
                    _unitOfWork.Save();

                    return RedirectToPage("/User/Food/Index");
                }
            }
            ViewData["Restaurants"] = new SelectList(_unitOfWork.Restaurant.GetAll().OrderByDescending(r => r.Date_Create), "Id", "Name");
            ViewData["mess"] = ".gif .png .jpg .webp .jpeg";
            return Page();
        }
    }
}
