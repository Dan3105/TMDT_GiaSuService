using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class AccountRepo : IAccountRepo
    {
        private readonly DvgsDbContext _context;
        public AccountRepo(DvgsDbContext context)
        {
            _context = context;
        }

        public void Create(Account entity)
        {
            _context.Accounts.Add(entity);
        }

        public async Task<Account?> GetByEmailOrPhone(string loginName)
        {
            string normLogin = loginName.ToLower();
            return (await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(p => 
                                (p.Email.ToLower() == normLogin || 
                                p.Phone.ToLower() == normLogin)
                                ));
        }

        public async Task<IEnumerable<Account>> GetAccountsByRoleId(int roleId)
        {
            var result = await _context.Accounts
                .Where(p => p.RoleId == roleId)
                .ToListAsync();

            return result;
        }

        public async Task<int?> GetRoleId(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(p => p.Name.ToLower() == roleName.ToLower());
            return role?.Id;
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Update(Account entity)
        {
            _context.Accounts.Update(entity);

        }

        public async Task<Account?> GetById(int id)
        {
            return (await _context.Accounts.FirstOrDefaultAsync(p => p.Id == id));
        }

        public void Delete(Account entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Tutor?> GetTutorByAccount(int accountId)
        {
            return await _context.Tutors.FirstOrDefaultAsync(p => p.AccountId == accountId);
        }

        public async Task<Employee?> GetEmployeeByAccount(int accountId)
        {
            return await _context.Employees.FirstOrDefaultAsync(p => p.AccountId == accountId);
        }

        public async Task<Customer?> GetCustomerByAccount(int accountId)
        {
            return await _context.Customers.FirstOrDefaultAsync(p => p.AccountId == accountId);
        }

        public async Task<bool> UpdatePassword(int accountId, string password)
        {
            Account? account = await GetById(accountId);
            if (account == null) return false;

            account.PasswordHash = password;
            Update(account);
            return await SaveChanges();
        }
    }
}
