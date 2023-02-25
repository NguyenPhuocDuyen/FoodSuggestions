using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.DataAccess.Repository;
using Project.DataAccess.Repository.IRepository;
using Project.Utility;
using System.Linq;
using System.Security.Claims;

namespace PRN221_Project.Pages.User.Food
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Project.Models.User> _userManager;
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration, IUnitOfWork unitOfWork, UserManager<Project.Models.User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _configuration = configuration;
        }

        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string PrivateFoodStore { get; set; }

        //public PaginatedList<Project.Models.FoodOfUser> PrivateFoods { get; set; }

        public PaginatedList<Project.Models.Food> FoodList { get; set; }

        /// <summary>
        /// get all food for user
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="pageIndex"></param>
        /// <param name="privateFoodStore">true if user chose privateFood</param>
        /// <returns></returns>
        public async Task OnGetAsync(string sortOrder, string currentFilter,
            string searchString, int? pageIndex, string privateFoodStore)
        {
            //CurrentSort = sortOrder;
            PrivateFoodStore = privateFoodStore;
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Project.Models.Food> foods = _unitOfWork.Food
                .GetAllAsync(includeProperties: "Restaurant,User", filter: f => f.IsBlackList == false)
                .OrderByDescending(f => f.Date_Create);

            if (PrivateFoodStore == "true")
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //get list food follow of user
                IQueryable<Project.Models.FoodOfUser> foodsOfUser = _unitOfWork.FoodOfUser
                    .GetAllAsync(includeProperties: "Food,User",
                                filter: f => f.Food.IsDeleted == false
                                        && f.UserId.Equals(userId))
                    .OrderByDescending(f => f.Food.Date_Create);

                foods = foods.Join(foodsOfUser.Where(f => f.IsDeleted == false), f => f.Id, fou => fou.FoodId, (f, fou) => f);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                foods = foods.Where(s => s.Name.Contains(searchString));
            }

            var pageSize = _configuration.GetValue("PageSize", 9);
            FoodList = await PaginatedList<Project.Models.Food>.CreateAsync(
                foods.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}