using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace GiaSuService.Controllers
{
    [Authorize(Policy = AppConfig.ADMINPOLICY)]
    public class StatisticController : Controller
    {
        private readonly IStatisticService _statisticService;
        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpGet]
        public async Task<IActionResult> StatisticAccount()
        {
            AccountStatisticsViewModel? data = await _statisticService.GetStatisticAccount();
            if(data is null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Hệ thống không lấy được data";
                return RedirectToAction("", "");
            }
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountData(int roleId, string fromDate, string toDate) 
        {
            try
            {
                DateOnly fromDateParse = DateOnly.Parse(fromDate);
                DateOnly toDateParse = DateOnly.Parse(toDate);
                DataTable? dataTable = await _statisticService.GetAccountCreateStatistic(roleId, fromDateParse, toDateParse);
                if (dataTable == null)
                {
                    throw new ArgumentNullException();
                }
                var data = new List<Dictionary<string, object>>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    data.Add(dict);
                }

                return Json(data);
            }
            catch
            {
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectStatistic(string fromDate, string toDate, int topK)
        {
            try
            {
                DateOnly fromDateParse = DateOnly.Parse(fromDate);
                DateOnly toDateParse = DateOnly.Parse(toDate);
                DataTable? dataTable = await _statisticService.GetRequestCreated(fromDateParse, toDateParse);
                if (dataTable == null)
                {
                    throw new ArgumentNullException();
                }
                var data = new List<Dictionary<string, object>>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    data.Add(dict);
                }

                TutorRequestStatisticsViewModel statisticsRequest = await _statisticService.GetStatisticRequest(fromDateParse, toDateParse, topK);
                
                return Json(new {chart=data, statis=statisticsRequest});
            }
            catch
            {
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        public IActionResult StatisticTutorRequest()
        {
            return View();
        }

        public IActionResult StatisticProfit() {
            return View();
        }
    }
}
