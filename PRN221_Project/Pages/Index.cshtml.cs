using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace PRN221_Project.Pages
{    
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int NumberOfFollow { get; set; }
        public int NumberOfUsers { get; set; }
        public int NumberOfFoods { get; set; }
        public int NumberOfReport { get; set; }

        //public List<Project.Models.FoodOfUser> FoodOfUserSortFollows { get; set; } = default!;
        public List<Project.Models.Food> Foods { get; set; } = default!;

        /// <summary>
        /// get all info food for index page
        /// </summary>
        public void OnGetAsync()
        {
            //get all follow food of all user
            var foodOfUsersIQ = _unitOfWork.FoodOfUser.GetAllAsync(
                filter: f => f.IsDeleted == false).ToList();
            NumberOfFollow = foodOfUsersIQ.Count;

            //get all report
            var foodReportIQ = _unitOfWork.Report.GetAllAsync().ToList();
            NumberOfReport = foodReportIQ.Count;

            //get all user in web
            var usersIQ = _unitOfWork.User.GetAllAsync().ToList();
            NumberOfUsers = usersIQ.Count;

            //get all food
            var foodsIQ = _unitOfWork.Food.
                GetAllAsync(includeProperties: "Restaurant",
                filter: f => f.IsBlackList == false).ToList();
            NumberOfFoods = foodsIQ.Count;

            //get new food to dispplay (4)
            Foods = foodsIQ.OrderByDescending(f=> f.Date_Create).Take(4).ToList();
        }
    }
}