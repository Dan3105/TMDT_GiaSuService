using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly TmdtDvgsContext _context;
        public AccountRepository(TmdtDvgsContext context)
        {
            _context = context;
        }

        public IQueryable<Account> All()
        {
            return _context.Accounts;
        }

        public async Task<bool> Create(Account entity)
        {
            try
            {
                _context.Accounts.Add(entity);
                return await SaveChanges();
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<Account> GetByEmail(string email)
        {
            return (await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower()))!;
        }

        public async Task<IEnumerable<Account>> GetAccountsByRoleId(int roleId)
        {
            var result = await _context.Accounts
                .Where(p => p.Roleid == roleId)
                .ToListAsync();

            return result;
        }

        public async Task<int?> GetRoleId(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(p => p.Rolename.ToLower() == roleName.ToLower());
            return role?.Id;
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> Update(Account entity)
        {
            try
            {
                _context.Accounts.Update(entity);
                return await SaveChanges();
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<Account> GetById(int id)
        {
            Account acc = (await _context.Accounts.FirstOrDefaultAsync(p => p.Id == id))!;
            return acc;
        }
    }
}
