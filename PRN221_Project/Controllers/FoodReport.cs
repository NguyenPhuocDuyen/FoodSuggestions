using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using System.Text.Json;

namespace PRN221_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodReport : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FoodReport(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// get all report
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string getAll()
        {
            List<Report> reportList = new List<Report>();
            reportList = _unitOfWork.Report
                .GetAll(includeProperties: "User,Food", filter: r => r.Status == 0)
                .OrderByDescending(r => r.Date)
                .ToList();
            return JsonSerializer.Serialize<List<Report>>(reportList);
        }

        /// <summary>
        /// accept report
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="foodID"></param>
        [HttpPost("Accept")]
        public void accept(string userID, int foodID)
        {
            List<Report> reportList = new List<Report>();
            var report = _unitOfWork.Report
                .GetFirstOrDefault(filter: r => r.Status == 0
                && r.FoodId == foodID
                && r.UserId == userID);
            //check null
            if (report != null)
            {
                //accept report and update Reason for food
                report.Status = 2;
                _unitOfWork.Report.Update(report);

                var food = _unitOfWork.Food.GetFirstOrDefault(filter: f => f.IsDeleted == false && f.Id == foodID);

                var foodIsBlackList = food.IsBlackList;

                // if food was reported once
                if (foodIsBlackList == true)
                {
                    //check food not delete
                    if (!food.IsDeleted)
                    {
                        food.Reason += ".\n Lý do thứ 2: " + report.Reason;
                        food.IsDeleted = true;

                        //set all report of food to approved
                        var foodListReport = _unitOfWork.Report.GetAll(f => f.FoodId == foodID).ToList();
                        foreach (var item in foodListReport)
                        {
                            item.Status = 2;
                            _unitOfWork.Report.Update(item);
                        }
                    }
                }
                else
                {
                    food.Reason = report.Reason;
                    food.IsBlackList = true;
                }
                _unitOfWork.Food.Update(food);
                _unitOfWork.Save();
            }
        }

        /// <summary>
        /// Reject report
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="foodID"></param>
        [HttpPost("Reject")]
        public void reject(string userID, int foodID)
        {
            var report = _unitOfWork.Report.
               GetFirstOrDefault(filter: r => r.UserId == userID && r.FoodId == foodID);
            if (report != null)
            {
                report.Status = 1;
                _unitOfWork.Report.Update(report);
                _unitOfWork.Save();
            }
        }
    }
}
