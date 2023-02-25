using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using System.Security.Claims;

namespace PRN221_Project.Pages.User.Food
{
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Project.Models.Food Food { get; set; } = default!;
        public int Follow { get; set; }
        public int FoodEaten { get; set; }
        public List<Feedback> Feedbacks { get; set; }

        /// <summary>
        /// get info food by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public  IActionResult OnGet(int? id)
        {
            if (id == null || _unitOfWork.Food.GetAll() == null)
            {
                return NotFound();
            }
            var food = _unitOfWork.Food.GetFirstOrDefault(
                includeProperties: "Restaurant,User",
                filter: f => f.Id == id
                && f.IsDeleted == false);

            if (food == null)
            {
                return NotFound();
            }
            var followOfFood = _unitOfWork.FoodOfUser.GetAll(
                filter: f => f.FoodId == id && f.IsDeleted == false).ToList();

            var foodEatens = _unitOfWork.FoodEaten.GetAll(
                filter: f => f.FoodId == id).ToList();

            var feedbackOfFood = _unitOfWork.Feedback.GetAll(
                includeProperties: "User",
                filter: f => f.FoodId == id).OrderByDescending(f => f.Date).ToList();

            Food = food;
            Follow = followOfFood.Count;
            FoodEaten = foodEatens.Count;
            Feedbacks = feedbackOfFood;

            // check follow of account login and eaten food
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var checkFOU = followOfFood.Where(f => f.UserId.Equals(userId)).FirstOrDefault();
            if (checkFOU != null)
            {
                ViewData["fou"] = "true";
            }

            var checkFE = foodEatens.Where(f => f.UserId.Equals(userId)).FirstOrDefault();
            if (checkFE != null)
            {
                ViewData["fe"] = "true";
            }

            return Page();
        }

        public Feedback Feedback { get; set; } = default!;
        /// <summary>
        /// feed back food
        /// </summary>
        /// <param name="foodId"></param>
        /// <param name="star"></param>
        /// <param name="description"></param>
        /// <param name="isReport"></param>
        /// <returns></returns>
        public IActionResult OnPost(int foodId, float star, string description, bool isReport)
        {
            description = description ?? "Không có nhận xét";
            var food = _unitOfWork.Food.GetFirstOrDefault(
                filter: f => f.Id == foodId
                && f.IsDeleted == false);

            if (food == null)
            {
                return NotFound();
            }
            //get user id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //check user want to report food
            if (!isReport) // feedback
            {
                var checkFeedback = _unitOfWork.Feedback
                    .GetFirstOrDefault(filter: f => f.UserId == userId && f.FoodId == foodId);
                // feedback already exists
                // update if exists 
                // add if don't exists
                if (checkFeedback != null) 
                {
                    checkFeedback.Description = description;
                    checkFeedback.Star = star;
                    checkFeedback.Date = DateTime.Now;
                    _unitOfWork.Feedback.Update(checkFeedback);
                }
                else
                {
                    Feedback fb = new Feedback();
                    fb.UserId = userId;
                    fb.FoodId = foodId;
                    fb.Description = description;
                    fb.Star = star;
                    _unitOfWork.Feedback.Add(fb);
                }
            }
            else //is report
            {
                //check report exists
                var checkReport = _unitOfWork.Report
                    .GetFirstOrDefault(filter: f => f.UserId == userId && f.FoodId == foodId);
                // report already exists
                // update if exists 
                // add if don't exists
                if (checkReport != null)
                {
                    checkReport.Reason = description;
                    checkReport.Date = DateTime.Now;
                    checkReport.Status = 0;
                    _unitOfWork.Report.Update(checkReport);
                }
                else
                {
                    Report rp = new Report();
                    rp.UserId = userId;
                    rp.FoodId = foodId;
                    rp.Reason = description;
                    rp.Status = 0;
                    _unitOfWork.Report.Add(rp);
                }
            }
            _unitOfWork.Save();

            return Redirect("./Details?id=" + foodId);
        }

        /// <summary>
        /// Marked as eat this food
        /// </summary>
        /// <param name="foodId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostEaten(int foodId)
        {
            //get user id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //find food
            var Checkfood = _unitOfWork.Food.
                GetAll(filter: f => f.Id == foodId && f.IsDeleted == false).FirstOrDefault();

            if (Checkfood == null)
            {
                return NotFound();
            }
            //check eaten
            var found = _unitOfWork.FoodEaten
                .GetAll(filter: f => f.UserId == userId && f.FoodId == foodId).FirstOrDefault();

            if (found == null)
            {
                FoodEaten foodEaten = new FoodEaten();
                foodEaten.UserId = userId;
                foodEaten.FoodId = foodId;
                _unitOfWork.FoodEaten.Add(foodEaten);
                _unitOfWork.Save();
            }
            return Redirect("./Details?id=" + foodId);
        }

        /// <summary>
        /// mark as follow this food
        /// </summary>
        /// <param name="foodId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostFollow(int foodId, string follow)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //find food
            var Checkfood = _unitOfWork.Food.
                GetAll(filter: f => f.Id == foodId && f.IsDeleted == false).FirstOrDefault();
            if (Checkfood == null)
            {
                return NotFound();
            }
            //check exists
            var found = _unitOfWork.FoodOfUser
                .GetAll(filter: f => f.UserId == userId && f.FoodId == foodId).FirstOrDefault();
            // add new follow
            if (found == null)
            {
                FoodOfUser foodOfU = new FoodOfUser();
                foodOfU.UserId = userId;
                foodOfU.FoodId = foodId;
                foodOfU.IsDeleted = false;
                _unitOfWork.FoodOfUser.Add(foodOfU);
            }
            else
            {
                // follow or unfollow
                if (follow.Equals("true"))
                {
                    //state follow
                    found.IsDeleted = false;
                } else
                {
                    found.IsDeleted = true;
                }
                _unitOfWork.FoodOfUser.Update(found);
            }
            _unitOfWork.Save();

            return Redirect("./Details?id=" + foodId);
        }
    }
}
