using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Services;
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
        public IActionResult StatisticAccount()
        {
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAnalystForEachRole()
        {
            AccountStatisticsViewModel? data = await _statisticService.GetStatisticAccount();
            if (data is null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Hệ thống không lấy được data";
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountData(string type, string fromDate, string toDate) 
        {
            try
            {
                AccountRegisterStatisticsViewModel? data = null;
                if(!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate)){
                    DateOnly fromDateParse = DateOnly.Parse(fromDate);
                    DateOnly toDateParse = DateOnly.Parse(toDate);
                    data = await _statisticService.GetAccountCreateStatistic(type, fromDateParse, toDateParse);
                }
                else
                {
                    data = await _statisticService.GetAccountCreateStatistic(type, DateOnly.MinValue, DateOnly.MinValue);
                }
                if (data == null)
                {
                    throw new ArgumentNullException();
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
                DataTable? dataTable = null; //await _statisticService.GetRequestCreated(fromDateParse, toDateParse);
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

                TutorRequestStatisticsViewModel statisticsRequest = await _statisticService.GetStatisticRequest();
                
                return Json(new {chart=data, statis=statisticsRequest});
            }
            catch
            {
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        [HttpGet]
        public async Task<IActionResult> StatisticTutorRequest()
        {
            TutorRequestStatisticsViewModel viewModel = await _statisticService.GetStatisticRequest();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> StatisticTutorRequestCreate(string type, string fromDate, string toDate)
        {
            try
            {
                TutorRequestStatisticCreateViewModel? data = null;
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    DateOnly fromDateParse = DateOnly.Parse(fromDate);
                    DateOnly toDateParse = DateOnly.Parse(toDate);
                    data = await _statisticService.GetRequestCreated(type, fromDateParse, toDateParse);
                }
                else
                {
                    data = await _statisticService.GetRequestCreated(type, DateOnly.MinValue, DateOnly.MinValue);
                }
                if (data == null)
                {
                    throw new ArgumentNullException();
                }

                return Json(data);
            }
            catch
            {
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        [HttpGet]
        public IActionResult StatisticProfit() {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetChartProfitByDate(string type, string fromDate, string toDate)
        {
            try
            {
                if(!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    DateOnly fromDateParse = DateOnly.Parse(fromDate);
                    DateOnly toDateParse = DateOnly.Parse(toDate);
                    TransactionStatisticByDateViewModel? data = await _statisticService.GetTransactionCreated(type, fromDateParse, toDateParse);
                    return Json(data);
                }
                else
                {
                    TransactionStatisticByDateViewModel? data = await _statisticService.GetTransactionCreated(type, DateOnly.MinValue, DateOnly.MinValue);
                    return Json(data);
                }
            }
            catch
            {
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChartDashboard()
        {
            try
            {
                TransactionStatisticsViewModel? statisticsRequest = await _statisticService.GetStatisticTranssaction();
                if(statisticsRequest == null) {
                    throw new NullReferenceException();
                }
                return Json(statisticsRequest);
            }
            catch
            {
                return StatusCode(500, "Lỗi hệ thống");
            }
        }
    }
}
