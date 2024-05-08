using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Npgsql;
using System.Data;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace GiaSuService.Repository
{
    public class StatisticRepo : IStatisticRepo
    {
        private readonly IConfiguration _configuration;
        private readonly DvgsDbContext _context;
        public StatisticRepo(IConfiguration configuration, DvgsDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private async Task<Dictionary<int, string>> GetRoles()
        {
            Dictionary<int, string> rolesDictionary = await _context.Roles
            .ToDictionaryAsync(r => r.Id, r => r.Name);

            return rolesDictionary;
        }

        public async Task<AccountRegisterStatisticsViewModel?> QueryStatisticAccount(string typeDate, DateOnly fromDate, DateOnly toDate)
        {
            try
            {
                AccountRegisterStatisticsViewModel response = new AccountRegisterStatisticsViewModel();
                var query = _context.Accounts.Select(p => new { CreateDate = p.CreateDate, p.Role.Name });

                if (typeDate == "this_month")
                {
                    var get_month = DateAndTime.Month(DateTime.Now);
                    var get_year = DateAndTime.Month(DateTime.Now);
                    DateTime start_date = new DateTime(get_year, get_month, 1);
                    DateTime next_date = new DateTime(get_year, get_month + 1, 1).AddDays(-1);

                    var register = await query
                                            .Where(p => p.CreateDate >= start_date && p.CreateDate <= next_date)
                                            .Select(p => new {CreateDateOnly=DateOnly.FromDateTime(p.CreateDate), p.Name})
                                            .GroupBy(p => p.CreateDateOnly)
                                            .Select(p => new
                                            {
                                                CreatedDate = p.Key,
                                                RoleCounts = p
                                                    .GroupBy(account => account.Name) // Within each date group, group by Role
                                                    .Select(roleGroup => new
                                                    {
                                                        Role = roleGroup.Key,
                                                        Count = roleGroup.Count() // Count the number of accounts in each role group
                                                    })
                                            })
                                            .ToDictionaryAsync(
                                                p => p.CreatedDate,
                                                p => p.RoleCounts.ToDictionary(
                                                    p => p.Role,
                                                    p => p.Count
                                                    )
                                            );

                    response.jsonRegisterStatisc = JsonConvert.SerializeObject(register);
                    return response;
                }

                if (typeDate == "this_week")
                {
                    var get_week = DateAndTime.Weekday(DateTime.Now); //Min: 0, Max: 6
                    var diff_date_start = DateTime.Now.AddDays(- get_week + 1); // 4: -> 5
                    var diff_date_end = DateTime.Now.AddDays(6 - get_week); // 

                    var start_date = diff_date_start;
                    var end_date = diff_date_end.AddDays(1).AddSeconds(-1);

                    var register = await query
                                            .Where(p => p.CreateDate >= start_date && p.CreateDate <= end_date)
                                            .Select(p => new { CreateDateOnly = DateOnly.FromDateTime(p.CreateDate), p.Name })
                                            .GroupBy(p => p.CreateDateOnly)
                                            .Select(p => new
                                            {
                                                CreatedDate = p.Key,
                                                RoleCounts = p
                                                .GroupBy(account => account.Name) // Within each date group, group by Role
                                                .Select(roleGroup => new
                                                {
                                                    Role = roleGroup.Key,
                                                    Count = roleGroup.Count() // Count the number of accounts in each role group
                                                })
                                            })
                                        .ToDictionaryAsync(
                                            p => p.CreatedDate,
                                            p => p.RoleCounts.ToDictionary(
                                                p => p.Role,
                                                p => p.Count
                                                )
                                        );

                    // Map day of the week to its name
                    var dayOfWeekNames = new Dictionary<int, string>
                    {
                        { 0, "Sunday" },
                        { 1, "Monday" },
                        { 2, "Tuesday" },
                        { 3, "Wednesday" },
                        { 4, "Thursday" },
                        { 5, "Friday" },
                        { 6, "Saturday" }
                    };

                    var registerWithWeekDayName = register
                        .ToDictionary(
                            kvp => dayOfWeekNames[(int)kvp.Key.DayOfWeek],
                            kvp => kvp.Value
                        );

                    response.jsonRegisterStatisc = JsonConvert.SerializeObject(registerWithWeekDayName);
                    return response;
                }

                if (typeDate == "custom")
                {
                    var start_fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                    var next_toDate =  new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                    var register = await query
                                        .Where(p => p.CreateDate >= start_fromDate && p.CreateDate <= next_toDate)
                                        .Select(p => new { CreateDateOnly = DateOnly.FromDateTime(p.CreateDate), p.Name })
                                        .GroupBy(p => p.CreateDateOnly)
                                        .Select(p => new
                                        {
                                            CreatedDate = p.Key,
                                            RoleCounts = p
                                                .GroupBy(account => account.Name) // Within each date group, group by Role
                                                .Select(roleGroup => new
                                                {
                                                    Role = roleGroup.Key,
                                                    Count = roleGroup.Count() // Count the number of accounts in each role group
                                                })
                                        })
                                        .ToDictionaryAsync(
                                            p => p.CreatedDate,
                                            p => p.RoleCounts.ToDictionary(
                                                p => p.Role,
                                                p => p.Count
                                                )
                                        );

                    response.jsonRegisterStatisc = JsonConvert.SerializeObject(register);
                    return response;
                }

                return null;

            }
            catch
            {
                return null;
            }

        }

        public async Task<AccountStatisticsViewModel?> GetAccountsCount()
        {
            int? tutorId = (await _context.Roles.FirstOrDefaultAsync(p => p.Name == AppConfig.TUTORROLENAME))?.Id;
            int? customerId = (await _context.Roles.FirstOrDefaultAsync(p => p.Name == AppConfig.CUSTOMERROLENAME))?.Id;
            int? employeeId = (await _context.Roles.FirstOrDefaultAsync(p => p.Name == AppConfig.EMPLOYEEROLENAME))?.Id;
            if (tutorId is null || customerId is null || employeeId is null)
            {
                return null;
            }

            AccountStatisticsViewModel statistic = new AccountStatisticsViewModel();
            var queryTutor = _context.Accounts
                .Select(p => new { p.RoleId, p.LockEnable })
                .Where(p => p.RoleId == (int)tutorId)
                .GroupBy(p => p.LockEnable);
            statistic.jsonTutorStatisc = JsonConvert.SerializeObject(new StatisticViewModel
            {
                labels = await queryTutor.Select(g => g.Key.ToString()).ToListAsync(),
                data = await queryTutor.Select(g => g.Count()).ToListAsync(),
            });

            var queryEmp = _context.Accounts
                .Select(p => new { p.RoleId, p.LockEnable })
                .Where(p => p.RoleId == (int)employeeId)
                .GroupBy(p => p.LockEnable);
            statistic.jsonEmployeeStatisc = JsonConvert.SerializeObject(new StatisticViewModel
            {
                labels = await queryEmp.Select(g => g.Key.ToString()).ToListAsync(),
                data = await queryEmp.Select(g => g.Count()).ToListAsync(),
            });

            var queryCustomer = _context.Accounts
                .Select(p => new { p.RoleId, p.LockEnable })
                .Where(p => p.RoleId == (int)customerId)
                .GroupBy(p => p.LockEnable);
            statistic.jsonCustomerStatisc = JsonConvert.SerializeObject(new StatisticViewModel
            {
                labels = await queryCustomer.Select(g => g.Key.ToString()).ToListAsync(),
                data = await queryCustomer.Select(g => g.Count()).ToListAsync(),
            });

            var queryTutorStatus = _context.Tutors
                .Select(p => new { p.IsActive, p.Status.Name });
            var queryTutorStatusAccount = queryTutorStatus.GroupBy(p => p.Name);
            statistic.jsonTutorStatusStatisc = JsonConvert.SerializeObject(new StatisticViewModel
            {
                labels = await queryTutorStatusAccount.Select(g => g.Key.ToString()).ToListAsync(),
                data = await queryTutorStatusAccount.Select(g => g.Count()).ToListAsync()
            });

            var queryTutorStatusActive = queryTutorStatus
                .Where(p => p.Name.Equals(AppConfig.RegisterStatus.APPROVAL.ToString().ToLower()))
                .GroupBy(p => p.IsActive);
            statistic.jsonTutorActiveStatisc = JsonConvert.SerializeObject(new StatisticViewModel
            {
                labels = await queryTutorStatusActive.Select(g => g.Key.ToString()!).ToListAsync(),
                data = await queryTutorStatusActive.Select(g => g.Count()).ToListAsync()
            });
            return statistic;
        }

        public async Task<DataTable?> QueryStatisticRequestsCreate(DateOnly fromDate, DateOnly toDate)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //string connString = "Host=myserver;Port=5432;Username=myuser;Password=mypass;Database=mydatabase;";
                string? connString = _configuration.GetConnectionString(AppConfig.connection_string);
                if (connString == null)
                {
                    throw new InvalidOperationException();
                }
                string query = $"select * from get_requests('{fromDate}', '{toDate}')";

                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    await conn.OpenAsync();
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn))
                    {
                        await Task.Run(() => da.Fill(dataTable));
                    }
                }

                return dataTable;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TutorRequestStatisticsViewModel> QueryStatisticRequests(DateOnly fromDate, DateOnly toDate, int topK)
        {

            var queryable = _context.RequestTutorForms
                .Select(p => new { p.CreateDate, p.Status.Name, p.Status.Id, SubjectName = p.Subject.Name })
                .Where(p => p.CreateDate >= new DateTime(fromDate.Year, fromDate.Month, fromDate.Day)
                                                         && p.CreateDate <= new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59));
            var result = new TutorRequestStatisticsViewModel();
            result.TotalCreated = await queryable.CountAsync();
            result.TotalPending = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.PENDING.ToString().ToLower())).CountAsync();
            result.TotalApproval = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.APPROVAL.ToString().ToLower())).CountAsync();
            result.TotalDeny = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.DENY.ToString().ToLower())).CountAsync();
            result.TotalHandover = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.HANDOVER.ToString().ToLower())).CountAsync();
            result.TotalCancel = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.CANCEL.ToString().ToLower())).CountAsync();

            try
            {
                var resultList = await queryable.ToListAsync();
                result.TopKPopular = resultList
                                        .GroupBy(p => p.SubjectName)
                                        .OrderByDescending(p => p.Count())
                                        .Take(topK)
                                        .ToDictionary(p => p.Key, g => g.Count());

            }
            catch (Exception)
            {

            }

            return result;
        }

        public async Task<TransactionStatisticsViewModel> QueryStatisticTransactions(DateOnly fromDate, DateOnly toDate)
        {
            try
            {
                var queryable = _context.TransactionHistories
                .Select(p => new { p.CreateDate, p.PaymentAmount, p.Status, p.TypeTransaction })
                .Where(p => p.CreateDate >= new DateTime(fromDate.Year, fromDate.Month, fromDate.Day)
                                                         && p.CreateDate <= new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59));

                TransactionStatisticsViewModel result = new TransactionStatisticsViewModel()
                {
                    TotalTransactions = await queryable.CountAsync(),
                    TotalTransactionsCancel = await queryable.Where(p => p.Status.Name.Equals(AppConfig.TransactionStatus.CANCEL.ToString().ToLower())).CountAsync(),
                    TotalTransactionsPaid = await queryable.Where(p => p.Status.Name.Equals(AppConfig.TransactionStatus.PAID.ToString().ToLower())).CountAsync(),
                    TotalTransactionsRefund = await queryable.Where(p => p.TypeTransaction == false).CountAsync(),
                    TotalTransactionsDeposit = await queryable.Where(p => p.TypeTransaction == true).CountAsync(),
                };

                return result;
            }
            catch (Exception)
            {
                throw new Exception();
            }

        }

        public async Task<DataTable?> QueryChartDataTransactions(DateOnly fromDate, DateOnly toDate)
        {
            try
            {
                DataTable dataTable = new DataTable();
                string? connString = _configuration.GetConnectionString(AppConfig.connection_string);
                if (connString == null)
                {
                    throw new InvalidOperationException();
                }
                string query = $"select * from get_profit('{fromDate}', '{toDate}')";

                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    await conn.OpenAsync();
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn))
                    {
                        await Task.Run(() => da.Fill(dataTable));
                    }
                }

                return dataTable;
            }
            catch
            {
                return null;
            }
        }
    }
}
