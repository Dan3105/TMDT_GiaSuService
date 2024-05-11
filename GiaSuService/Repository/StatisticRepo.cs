using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                    var get_year = DateAndTime.Year(DateTime.Now);
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
                .Select(p => new { p.IsActive, p.Status.VietnameseName, p.Status.Name });
            var queryTutorStatusAccount = queryTutorStatus.GroupBy(p => p.VietnameseName);
            statistic.jsonTutorStatusStatisc = JsonConvert.SerializeObject(new StatisticViewModel
            {
                labels = await queryTutorStatusAccount.Select(g => g.Key).ToListAsync(),
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

        public async Task<TutorRequestStatisticsViewModel> QueryStatisticRequests()
        {

            try
            {
                var queryable = _context.RequestTutorForms
                    .Select(p => new { p.CreateDate, p.ExpiredDate, p.Status.Name, p.Status.Id, SubjectName = p.Subject.Name });
               
                var result = new TutorRequestStatisticsViewModel();
                //result.TotalCreated = await queryable.CountAsync();
                result.TotalPending = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.PENDING.ToString().ToLower()) && p.ExpiredDate > DateTime.Now).CountAsync();
                result.TotalApproval = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.APPROVAL.ToString().ToLower())).CountAsync();
                result.TotalDeny = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.DENY.ToString().ToLower())).CountAsync();
                result.TotalHandover = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.HANDOVER.ToString().ToLower())).CountAsync();
                result.TotalCancel = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.CANCEL.ToString().ToLower())).CountAsync();
                result.TotalExpired = await queryable.Where(p => p.Name.Equals(AppConfig.FormStatus.APPROVAL.ToString().ToLower()) && p.ExpiredDate <= DateTime.Now).CountAsync();

                return result;
            }
            catch (Exception)
            {
                return new TutorRequestStatisticsViewModel();
            }

        }

        public async Task<TutorRequestStatisticCreateViewModel?> QueryStatisticRequestsCreate(string type, DateOnly from, DateOnly to)
        {
            try
            {
                TutorRequestStatisticCreateViewModel result = new TutorRequestStatisticCreateViewModel();
                var query = _context.RequestTutorForms.Select(p => new { p.CreateDate, StatusName=p.Status.VietnameseName, p.ExpiredDate, 
                                                    SubjectName=p.Subject.Name});

                if (type == "this_month")
                {
                    var get_month = DateAndTime.Month(DateTime.Now);
                    var get_year = DateAndTime.Year(DateTime.Now);
                    DateTime start_date = new DateTime(get_year, get_month, 1);
                    DateTime next_date = new DateTime(get_year, get_month + 1, 1).AddDays(-1);

                    var queryThisMonth = query
                                            .Where(p => p.CreateDate >= start_date && p.CreateDate <= next_date);

                    var requestCreate = await queryThisMonth
                                            .Select(p => new { CreateDateOnly = DateOnly.FromDateTime(p.CreateDate) }).GroupBy(p => p.CreateDateOnly)
                                            .Select(p => new
                                            {
                                                CreatedDate = p.Key,
                                                Count = p.Count(),
                                            })
                                            .ToDictionaryAsync(
                                                p => p.CreatedDate,
                                                p => p.Count
                                            );

                    var requestStatus = await queryThisMonth
                                            .Select(p => new { p.StatusName }).GroupBy(p => p.StatusName)
                                            .Select(p => new
                                            {
                                                StatusName = p.Key,
                                                Count = p.Count()
                                            }).ToDictionaryAsync(
                                                p => p.StatusName,
                                                p => p.Count
                                            );

                    var requestSubject = await queryThisMonth
                                              .Select(p => new { p.SubjectName }).GroupBy(p => p.SubjectName)
                                                .Select(p => new
                                                {
                                                    SubjectName = p.Key,
                                                    Count = p.Count()
                                                }).ToDictionaryAsync(
                                                    p => p.SubjectName,
                                                    p => p.Count
                                                );
                    result.jsonTutorRequestCreate = JsonConvert.SerializeObject(requestCreate);
                    result.jsonTutorRequestStatus = JsonConvert.SerializeObject(requestStatus);
                    result.jsonTutorRequestSubject = JsonConvert.SerializeObject(requestSubject);
                    return result;
                }

                if (type == "this_week")
                {
                    var get_week = DateAndTime.Weekday(DateTime.Now); //Min: 0, Max: 6
                    var diff_date_start = DateTime.Now.AddDays(-get_week + 1); // 4: -> 5
                    var diff_date_end = DateTime.Now.AddDays(6 - get_week); // 

                    var start_date = diff_date_start;
                    var end_date = diff_date_end.AddDays(1).AddSeconds(-1);
                    var queryThisMonth = query
                                            .Where(p => p.CreateDate >= start_date && p.CreateDate <= end_date);

                    var requestCreate = await queryThisMonth
                                            .Select(p => new { CreateDateOnly = DateOnly.FromDateTime(p.CreateDate) }).GroupBy(p => p.CreateDateOnly)
                                            .Select(p => new
                                            {
                                                CreatedDate = p.Key,
                                                Count = p.Count(),
                                            })
                                            .ToDictionaryAsync(
                                                p => p.CreatedDate,
                                                p => p.Count
                                            );

                    var requestStatus = await queryThisMonth
                                            .Select(p => new { p.StatusName }).GroupBy(p => p.StatusName)
                                            .Select(p => new
                                            {
                                                StatusName = p.Key,
                                                Count = p.Count()
                                            }).ToDictionaryAsync(
                                                p => p.StatusName,
                                                p => p.Count
                                            );

                    var requestSubject = await queryThisMonth
                                              .Select(p => new { p.SubjectName }).GroupBy(p => p.SubjectName)
                                                .Select(p => new
                                                {
                                                    SubjectName = p.Key,
                                                    Count = p.Count()
                                                }).ToDictionaryAsync(
                                                    p => p.SubjectName,
                                                    p => p.Count
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

                    var registerWithWeekDayName = requestCreate
                        .ToDictionary(
                            kvp => dayOfWeekNames[(int)kvp.Key.DayOfWeek],
                            kvp => kvp.Value
                        );

                    result.jsonTutorRequestCreate = JsonConvert.SerializeObject(registerWithWeekDayName);
                    result.jsonTutorRequestStatus = JsonConvert.SerializeObject(requestStatus);
                    result.jsonTutorRequestSubject = JsonConvert.SerializeObject(requestSubject);
                    //result.jsonTutorCreate = JsonConvert.SerializeObject(registerWithWeekDayName);
                    return result;
                }

                if (type == "custom")
                {
                    var start_fromDate = new DateTime(from.Year, from.Month, from.Day);
                    var next_toDate = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);
                    var queryCustom = query
                                        .Where(p => p.CreateDate >= start_fromDate && p.CreateDate <= next_toDate);

                    var requestCreate = await queryCustom
                                            .Select(p => new { CreateDateOnly = DateOnly.FromDateTime(p.CreateDate) }).GroupBy(p => p.CreateDateOnly)
                                            .Select(p => new
                                            {
                                                CreatedDate = p.Key,
                                                Count = p.Count(),
                                            })
                                            .ToDictionaryAsync(
                                                p => p.CreatedDate,
                                                p => p.Count
                                            );

                    var requestStatus = await queryCustom
                                            .Select(p => new { p.StatusName }).GroupBy(p => p.StatusName)
                                            .Select(p => new
                                            {
                                                StatusName = p.Key,
                                                Count = p.Count()
                                            }).ToDictionaryAsync(
                                                p => p.StatusName,
                                                p => p.Count
                                            );

                    var requestSubject = await queryCustom
                                              .Select(p => new { p.SubjectName }).GroupBy(p => p.SubjectName)
                                                .Select(p => new
                                                {
                                                    SubjectName = p.Key,
                                                    Count = p.Count()
                                                }).ToDictionaryAsync(
                                                    p => p.SubjectName,
                                                    p => p.Count
                                                );
                    result.jsonTutorRequestCreate = JsonConvert.SerializeObject(requestCreate);
                    result.jsonTutorRequestStatus = JsonConvert.SerializeObject(requestStatus);
                    result.jsonTutorRequestSubject = JsonConvert.SerializeObject(requestSubject);
                    return result;
                }
            }
            catch
            {

            }
            return null;
        }

        public async Task<TransactionStatisticByDateViewModel?> QueryChartTransactionsByDate(string typeDate, DateOnly fromDate, DateOnly toDate)
        {
            try
            {
                TransactionStatisticByDateViewModel response = new TransactionStatisticByDateViewModel();
                var query = _context.TransactionHistories.Select(p => new {p.CreateDate, p.TypeTransaction, p.Status.VietnameseName, p.Status.Name, p.PaymentAmount });

                if (typeDate == "this_month")
                {
                    var get_month = DateAndTime.Month(DateTime.Now);
                    var get_year = DateAndTime.Year(DateTime.Now);
                    DateTime start_date = new DateTime(get_year, get_month, 1);
                    DateTime next_date = new DateTime(get_year, get_month + 1, 1).AddDays(-1);
                    var queryThisMonth = query
                                            .Where(p => p.CreateDate >= start_date && p.CreateDate <= next_date
                                                        );
                    response.JsonTransactionType = JsonConvert.SerializeObject(await queryThisMonth.Select(p => p.TypeTransaction)
                        .GroupBy(p => p)
                        .ToDictionaryAsync(p => p.Key, p => p.Count()));

                    response.JsonTransactionStatus = JsonConvert.SerializeObject(await queryThisMonth.Select(p => p.VietnameseName)
                        .GroupBy(p => p)
                        .ToDictionaryAsync(p => p.Key, p => p.Count()));

                    var queryPaided = queryThisMonth.Where(p => p.Name.Equals(AppConfig.TransactionStatus.PAID.ToString().ToLower()));

                    response.TotalDeposit = await queryThisMonth.Where(p => p.TypeTransaction).SumAsync(p => p.PaymentAmount);
                    response.TotalRefund = await queryThisMonth.Where(p => !p.TypeTransaction).SumAsync(p => p.PaymentAmount);
                    response.TotalProfit = response.TotalDeposit - response.TotalRefund;

                    response.JsonTransaction = JsonConvert.SerializeObject(await queryPaided
                                                .Select(p => new { CreateDateOnly = DateOnly.FromDateTime(p.CreateDate), p.TypeTransaction, p.PaymentAmount })
                                                .GroupBy(p =>p.CreateDateOnly )
                                                .Select(p => new
                                                {
                                                    p.Key,
                                                    Transaction = p
                                                    .GroupBy(g => g.TypeTransaction) // Within each date group, group by Role
                                                    .Select(g => new
                                                    {
                                                        Type = g.Key,
                                                        Sum = g.Sum(p => p.PaymentAmount) // Count the number of accounts in each role group
                                                    })
                                                }).ToDictionaryAsync(
                                                    p => p.Key, 
                                                    p => p.Transaction.ToDictionary(p => p.Type, p => p.Sum)
                                                    ));
                    

                    return response;
                }

                if (typeDate == "this_week")
                {
                    var get_week = DateAndTime.Weekday(DateTime.Now); //Min: 0, Max: 6
                    var diff_date_start = DateTime.Now.AddDays(-get_week + 1); // 4: -> 5
                    var diff_date_end = DateTime.Now.AddDays(6 - get_week); // 

                    var start_date = diff_date_start;
                    var end_date = diff_date_end.AddDays(1).AddSeconds(-1);

                    var queryThisMonth = query
                                            .Where(p => p.CreateDate >= start_date && p.CreateDate <= end_date
                                                        );
                    response.JsonTransactionType = JsonConvert.SerializeObject(await queryThisMonth.Select(p => p.TypeTransaction)
                        .GroupBy(p => p)
                        .ToDictionaryAsync(p => p.Key, p => p.Count()));

                    response.JsonTransactionStatus = JsonConvert.SerializeObject(await queryThisMonth.Select(p => p.VietnameseName)
                        .GroupBy(p => p)
                        .ToDictionaryAsync(p => p.Key, p => p.Count()));

                    var queryPaided = queryThisMonth.Where(p => p.Name.Equals(AppConfig.TransactionStatus.PAID.ToString().ToLower()));

                    response.TotalDeposit = await queryThisMonth.Where(p => p.TypeTransaction).SumAsync(p => p.PaymentAmount);
                    response.TotalRefund = await queryThisMonth.Where(p => !p.TypeTransaction).SumAsync(p => p.PaymentAmount);
                    response.TotalProfit = response.TotalDeposit - response.TotalRefund;

                    var result = await queryPaided
                                                .Select(p => new { CreateDateOnly = DateOnly.FromDateTime(p.CreateDate), p.TypeTransaction, p.PaymentAmount })
                                                .GroupBy(p => p.CreateDateOnly)
                                                .Select(p => new
                                                {
                                                    p.Key,
                                                    Transaction = p
                                                    .GroupBy(g => g.TypeTransaction) // Within each date group, group by Role
                                                    .Select(g => new
                                                    {
                                                        Type = g.Key,
                                                        Sum = g.Sum(p => p.PaymentAmount) // Count the number of accounts in each role group
                                                    })
                                                }).ToDictionaryAsync(
                                                    p => p.Key,
                                                    p => p.Transaction.ToDictionary(p => p.Type, p => p.Sum)
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

                    var registerWithWeekDayName = result
                        .ToDictionary(
                            kvp => dayOfWeekNames[(int)kvp.Key.DayOfWeek],
                            kvp => kvp.Value
                        );

                    response.JsonTransaction = JsonConvert.SerializeObject(registerWithWeekDayName);
                    return response;
                }

                if (typeDate == "custom")
                {
                    var start_fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                    var next_toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                    var queryThisMonth = query
                                            .Where(p => p.CreateDate >= start_fromDate && p.CreateDate <= next_toDate
                                                        );
                    response.JsonTransactionType = JsonConvert.SerializeObject(await queryThisMonth.Select(p => p.TypeTransaction)
                        .GroupBy(p => p)
                        .ToDictionaryAsync(p => p.Key, p => p.Count()));

                    response.JsonTransactionStatus = JsonConvert.SerializeObject(await queryThisMonth.Select(p => p.VietnameseName)
                        .GroupBy(p => p)
                        .ToDictionaryAsync(p => p.Key, p => p.Count()));

                    var queryPaided = queryThisMonth.Where(p => p.Name.Equals(AppConfig.TransactionStatus.PAID.ToString().ToLower()));

                    response.TotalDeposit = await queryThisMonth.Where(p => p.TypeTransaction).SumAsync(p => p.PaymentAmount);
                    response.TotalRefund = await queryThisMonth.Where(p => !p.TypeTransaction).SumAsync(p => p.PaymentAmount);
                    response.TotalProfit = response.TotalDeposit - response.TotalRefund;

                    response.JsonTransaction = JsonConvert.SerializeObject(await queryPaided
                                                .Select(p => new { CreateDateOnly = DateOnly.FromDateTime(p.CreateDate), p.TypeTransaction, p.PaymentAmount })
                                                .GroupBy(p => p.CreateDateOnly)
                                                .Select(p => new
                                                {
                                                    p.Key,
                                                    Transaction = p
                                                    .GroupBy(g => g.TypeTransaction) // Within each date group, group by Role
                                                    .Select(g => new
                                                    {
                                                        Type = g.Key,
                                                        Sum = g.Sum(p => p.PaymentAmount) // Count the number of accounts in each role group
                                                    })
                                                }).ToDictionaryAsync(
                                                    p => p.Key,
                                                    p => p.Transaction.ToDictionary(p => p.Type, p => p.Sum)
                                                    ));

                    return response;
                }

                return null;

            }
            catch
            {
                return null;
            }
        }
        
        public async Task<TransactionStatisticsViewModel?> QueryStatisticTransactions()
        {
            try
            {
                var query = _context.TransactionHistories.Select(p => new { p.CreateDate, p.TypeTransaction, p.Status.VietnameseName, p.Status.Name, p.PaymentAmount });
                TransactionStatisticsViewModel result = new TransactionStatisticsViewModel();
                var paid_query = query.Where(p => p.Name.Equals(AppConfig.TransactionStatus.PAID.ToString().ToLower()));
                result.TotalTransactions = await query.CountAsync();
                result.TotalDeposit = await paid_query.Where(p => p.TypeTransaction).SumAsync(p => p.PaymentAmount);
                result.TotalRefund = await paid_query.Where(p => !p.TypeTransaction).SumAsync(p => p.PaymentAmount);
                result.TotalProfit = result.TotalDeposit - result.TotalRefund;

                result.JsonTransactionType = JsonConvert.SerializeObject(
                                            await query.Select(p => p.TypeTransaction)
                                                    .GroupBy(p => p)
                                                    .Select(p => new { p.Key, Count = p.Count() })
                                                    .ToDictionaryAsync(p => p.Key, p => p.Count)
                                                    );

                result.JsonTransactionStatus = JsonConvert.SerializeObject(
                                            await query.Select(p => p.VietnameseName)
                                                    .GroupBy(p => p)
                                                    .Select(p => new { p.Key, Count = p.Count() })
                                                    .ToDictionaryAsync(p => p.Key, p => p.Count)
                                                    );

                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
