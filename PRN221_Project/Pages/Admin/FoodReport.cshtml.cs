using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using Project.DataAccess.Repository;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using Project.Utility;

namespace PRN221_Project.Pages.Admin
{
    [Authorize(Roles = SD.AdminRole)]
    public class FoodReportModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public FoodReportModel(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public PaginatedList<Report> Reports { get; set; } = default!;

        public async Task OnGetAsync(int? pageIndex)
        {
            Reports = await getListReport(pageIndex);
        }

        public async Task<IActionResult> OnPostDelete(int foodId, string userId, int? pageIndex)
        {
            var report = _unitOfWork.Report.
                GetFirstOrDefault(filter: r => r.UserId == userId && r.FoodId == foodId);
            if (report == null)
            {
                return NotFound();
            }
            report.Status = 1;
            _unitOfWork.Report.Update(report);
            _unitOfWork.Save();
            Reports = await getListReport(pageIndex);
            return Page();
        }

        public async Task<IActionResult> OnPostAccept(int foodId, string userId, int? pageIndex)
        {
            var report = _unitOfWork.Report.
                GetFirstOrDefault(filter: r => r.UserId == userId && r.FoodId == foodId);
            if (report == null)
            {
                return NotFound();
            }
            report.Status = 2;
            _unitOfWork.Report.Update(report);

            var food = _unitOfWork.Food.GetFirstOrDefault(filter: f => f.IsDeleted == false && f.Id == foodId);
            
            var foodIsBlackList = food.IsBlackList ;
            if (foodIsBlackList == true)
            {
                if (!food.IsDeleted)
                {
                    food.Reason += ".\n Lý do thứ 2: " + report.Reason;
                    food.IsDeleted = true;
                }
            }
            else
            {
                food.Reason = report.Reason;
                food.IsBlackList = true;
            }
            _unitOfWork.Food.Update(food);

            _unitOfWork.Save();
            Reports = await getListReport(pageIndex);
            return Page();
        }

        private async Task<PaginatedList<Report>> getListReport(int? pageIndex)
        {
            var reportIQ = _unitOfWork.Report.
                GetAllAsync(includeProperties: "User,Food", filter: r => r.Status == 0)
                .OrderByDescending(r => r.Date);

            var pageSize = _configuration.GetValue("PageSize", 9);
            return await PaginatedList<Project.Models.Report>.CreateAsync(
                reportIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
