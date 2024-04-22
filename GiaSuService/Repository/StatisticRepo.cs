using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

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

        public async Task<DataTable?> QueryStatisticAccount(int roleId, DateOnly fromDate, DateOnly toDate)
        {
            try
            {
                DataTable dataTable = new DataTable();
                string? connString = _configuration.GetConnectionString(AppConfig.connection_string);
                if (connString == null)
                {
                    throw new InvalidOperationException();
                }
                string query = $"select * from get_account_created({roleId}, '{fromDate}', '{toDate}')";

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
            catch {
                return null;
            }
            
        }

        public async Task<DataTable?> GetProfit(DateOnly fromDate, DateOnly toDate)
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

        public async Task<AccountStatisticsViewModel?> GetAccountsCount()
        {
            int? tutorId = (await _context.Roles.FirstOrDefaultAsync(p => p.Name == AppConfig.TUTORROLENAME))?.Id;
            int? customerId = (await _context.Roles.FirstOrDefaultAsync(p => p.Name == AppConfig.CUSTOMERROLENAME))?.Id;
            int? employeeId = (await _context.Roles.FirstOrDefaultAsync(p => p.Name == AppConfig.EMPLOYEEROLENAME))?.Id;
            if(tutorId is null || customerId is null || employeeId is null)
            {
                return null;
            }

            AccountStatisticsViewModel statistic = new AccountStatisticsViewModel();
            statistic.TotalTutor = _context.Accounts.Where(p => p.RoleId == (int)tutorId).Count();
            statistic.TotalCustomer = _context.Accounts.Where(p => p.RoleId == (int)customerId).Count();
            statistic.TotalEmployee = _context.Accounts.Where(p => p.RoleId == (int)employeeId).Count();
            statistic.ListRole = await GetRoles();
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
                .Select(p => new {p.CreateDate, p.Status.Name, p.Status.Id, SubjectName=p.Subject.Name})
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
    }
}
