using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DvgsDbContext _context;
        public AccountRepository(DvgsDbContext context)
        {
            _context = context;
        }

        public IQueryable<Account> All()
        {
            return _context.Accounts;
        }

        public void Create(Account entity)
        {
            _context.Accounts.Add(entity);
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
            //var role = await _context.Roles.FirstOrDefaultAsync(p => p.Rolename.ToLower() == roleName.ToLower());
            //return role?.Id;
            return 0;
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Update(Account entity)
        {
            _context.Accounts.Update(entity);

        }

        public async Task<Account> GetById(int id)
        {
            Account acc = (await _context.Accounts.FirstOrDefaultAsync(p => p.Id == id))!;
            return acc;
        }

        public void Delete(Account entity)
        {
            throw new NotImplementedException();
        }

    }
}
