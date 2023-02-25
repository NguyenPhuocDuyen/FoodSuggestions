// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using PRN221_Project.ViewModels;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using Project.Utility.Helpers.Validator;

namespace PRN221_Project.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileValidator _fileValidator;


        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager, IUnitOfWork unitOfWork, IWebHostEnvironment environment, IFileValidator fileValidator)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _fileValidator = fileValidator;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [BindProperty]
        public User UserInfo { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }
            //[Display(Name = "Họ của bạn")]
            //public string LastName { get; set; }
            //[Required(ErrorMessage = "Vui lòng điền tên")]
            //[Display(Name = "Tên của bạn")]
            //public string FirstName { get; set; }
            //[Display(Name = "Ngày sinh")]

            //[DataType(DataType.Date)]
            //public DateTime Birthday { get; set; } = DateTime.Parse("01/01/2001");
            //[Display(Name = "Địa chỉ")]
            //public string Address { get; set; }

            public IFormFile? ImageFile { get; set; }

        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            UserInfo = _unitOfWork.User.GetFirstOrDefault(x => x.Id == user.Id);


            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            else
            {
                if (Input.ImageFile is not null)
                {
                    if (!_fileValidator.IsValid(Input.ImageFile))
                    {
                        ModelState.AddModelError("ImageFile", "Hình ảnh không hợp lệ!");
                        await LoadAsync(user);
                        return Page();
                    }
                    else
                    {
                        var fileName = DateTime.Now.ToString("yyyymmddMMss") + "_" + Path.GetFileName(Input.ImageFile.FileName);
                        var folderPath = Path.Combine(_environment.WebRootPath, "images/Avatars");
                        var filePath = Path.Combine(folderPath, fileName);
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Input.ImageFile.CopyToAsync(fileStream);
                        }
                        user.UserImage = "/images/Avatars/" + fileName;
                    }
                }
                
                if (user.Address != UserInfo.Address)
                {
                    user.Address = UserInfo.Address;
                }

                if (user.FirstName != UserInfo.FirstName)
                {
                    user.FirstName = UserInfo.FirstName;
                }
                if (user.LastName != UserInfo.LastName)
                {
                    user.LastName = UserInfo.LastName;
                }
                if (user.Birthday != UserInfo.Birthday)
                {
                    user.Birthday = UserInfo.Birthday;
                }

                user.Date_Edit = DateTime.Now;


                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        StatusMessage = "Một lỗi không mong muốn đã xảy ra khi cập nhật số điện thoại";
                        return RedirectToPage();
                    }
                }

                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Thông tin đã được cập nhật";
                return RedirectToPage();
            }
        }
    }
}
