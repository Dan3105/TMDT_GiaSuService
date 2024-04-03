using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class ProfileRepo : IProfileRepo
    {
        private readonly DvgsDbContext _context;
        public ProfileRepo(DvgsDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetProfileId(int accountId, string roleName)
        {
            if (roleName.Equals(AppConfig.ADMINROLENAME.ToLower()) || roleName.Equals(AppConfig.EMPLOYEEROLENAME.ToLower()))
            {
                return (await _context.Employees.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Accountid == accountId))?.Id;

            }

            if (roleName.Equals(AppConfig.TUTORROLENAME.ToLower()))
            {
                return (await _context.Tutors.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Accountid == accountId))?.Id;
            }

            if (roleName.Equals(AppConfig.CUSTOMERROLENAME))
            {
                return (await _context.Customers.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Accountid == accountId))?.Id;
            }

            return null;
        }

        public async Task<Identitycard?> GetIdentitycard(string identityNumber)
        {
            return await _context.Identitycards.AsNoTracking().FirstOrDefaultAsync(x => x.Identitynumber == identityNumber);
        }

        public async Task<List<AccountListViewModel>> GetEmployeeList(int crrPage)
        {
            IQueryable<AccountListViewModel> query = _context.Employees
                .AsNoTracking()
                .Where(p => p.Account.Roleid != 1)
                .Select(p => new AccountListViewModel()
                {
                    Email = p.Account.Email,
                    FullName = p.Fullname,
                    Id = p.Id,
                    ImageUrl = p.Account.Avatar,
                    LockStatus = p.Account.Lockenable ?? false,
                })
                .OrderBy(p => p.Id)
                ;

            query = query.Skip(crrPage * AppConfig.ROWS_ACCOUNT_LIST)
                    .Take(AppConfig.ROWS_ACCOUNT_LIST);

            return await query.ToListAsync();
        }

        public async Task<ProfileViewModel?> GetEmployeeProfile(int accountId)
        {
            ProfileViewModel? profile = await _context.Employees
                .AsNoTracking()
                .Select(p => new ProfileViewModel()
                {
                    ProfileId = p.Id,
                    IdentityId = p.Identityid,
                    AccountId = p.Accountid,
                    Avatar = p.Account.Avatar,
                    Email = p.Account.Email,
                    BirthDate = p.Birth,
                    FullName = p.Fullname,
                    LockStatus = p.Account.Lockenable ?? false,
                    Phone = p.Account.Phone,
                    Gender = (p.Gender == "M" ? "Nam" : "Nữ"),
                    IdentityCard = p.Identity.Identitynumber,
                    FrontIdentityCard = p.Identity.Frontidentitycard,
                    BackIdentityCard = p.Identity.Backidentitycard,

                    AddressDetail = p.Addressdetail,
                    SelectedDistrictId = p.Districtid,
                    SelectedProvinceId = p.District.Provinceid,
                })
                .FirstOrDefaultAsync(p => p.AccountId == accountId);

            return profile;
        }

        public async Task<ProfileViewModel?> GetCustomerProfile(int accountId)
        {
            ProfileViewModel? profile = await _context.Customers
                .AsNoTracking()
                .Select(p => new ProfileViewModel()
                {
                    ProfileId = p.Id,
                    IdentityId = p.Identityid,
                    AccountId = p.Accountid,
                    Avatar = p.Account.Avatar,
                    Email = p.Account.Email,
                    BirthDate = p.Birth,
                    FullName = p.Fullname,
                    LockStatus = p.Account.Lockenable ?? false,
                    Phone = p.Account.Phone,
                    Gender = (p.Gender == "M" ? "Nam" : "Nữ"),
                    IdentityCard = p.Identity.Identitynumber,
                    FrontIdentityCard = p.Identity.Frontidentitycard,
                    BackIdentityCard = p.Identity.Backidentitycard,

                    AddressDetail = p.Addressdetail,
                    SelectedDistrictId = p.Districtid,
                    SelectedProvinceId = p.District.Provinceid,
                })
                .FirstOrDefaultAsync(p => p.AccountId == accountId);

            return profile;
        }

        public async Task<TutorProfileViewModel?> GetTutorProfile(int accountId)
        {
            var profile = await _context.Tutors
                .Select(tutor => new TutorProfileViewModel
                {
                    TutorId = tutor.Id,
                    AccountId = tutor.Accountid,
                    IdentityId = tutor.Identityid,
                    Email = tutor.Account.Email,
                    Phone = tutor.Account.Phone,
                    Lockenable = tutor.Account.Lockenable ?? false,
                    Createdate = DateOnly.FromDateTime((DateTime)tutor.Account.Createdate!),
                    LockStatus = tutor.Account.Lockenable ?? false,
                    Avatar = tutor.Account.Avatar,

                    Fullname = tutor.Fullname,
                    Gender = tutor.Gender == "M" ? "Nam" : "Nữ",
                    AddressDetail = tutor.Addressdetail,
                    SelectedDistrictId = tutor.Districtid,
                    SelectedProvinceId = tutor.District.Provinceid,

                    Area = tutor.Area,
                    College = tutor.College,
                    Academicyearfrom = tutor.Academicyearfrom,
                    Academicyearto = tutor.Academicyearto,
                    Additionalinfo = tutor.Additionalinfo,
                    Birth = tutor.Birth,

                    TypeTutor = tutor.Typetutor,

                    IdentityCard = tutor.Identity.Identitynumber,
                    FrontIdentityCard = tutor.Identity.Frontidentitycard,
                    BackIdentityCard = tutor.Identity.Backidentitycard,
                })
                .FirstOrDefaultAsync(p => p.AccountId == accountId);

            return profile;
        }

        public async Task<ResponseService> UpdateEmployeeProfile(ProfileViewModel profile)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Identitycard? identity = await _context.Identitycards.FindAsync(profile.IdentityId);
                    if (identity == null)
                    {
                        return new ResponseService { Success = false, Message = "Không tìm thấy CMND/CCCD" };
                    }
                    identity.Identitynumber = profile.IdentityCard;
                    identity.Backidentitycard = profile.BackIdentityCard;
                    identity.Frontidentitycard = profile.FrontIdentityCard;

                    Account? account = await _context.Accounts.FindAsync(profile.AccountId);
                    if (account == null)
                    {
                        return new ResponseService { Success = false, Message = "Không tìm thấy tài khoản" };
                    }
                    account.Email = profile.Email;
                    account.Phone = profile.Phone;
                    account.Avatar = profile.Avatar;
                    account.Lockenable = profile.LockStatus;

                    Employee? employee = await _context.Employees.FindAsync(profile.ProfileId);
                    if (employee == null)
                    {
                        return new ResponseService { Success = false, Message = "Không tìm thấy nhân viên" };
                    }
                    employee.Fullname = profile.FullName;
                    employee.Addressdetail = profile.AddressDetail;
                    employee.Gender = profile.Gender == "Nam" ? "M" : "F";
                    employee.Birth = profile.BirthDate;
                    employee.Districtid = profile.SelectedDistrictId;

                    _context.Identitycards.Update(identity);
                    _context.Accounts.Update(account);
                    _context.Employees.Update(employee);
                    int result = _context.SaveChanges();
                    transaction.Commit();
                    if (result > 0)
                    {
                        return new ResponseService { Success = true, Message = "Cập nhật thành công" };
                    }
                    else
                    {
                        return new ResponseService { Success = false, Message = "Cập nhật không thành công" };
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return new ResponseService { Success = false, Message = "Cập nhật không thành công" };
        }

        public async Task<ResponseService> UpdateCustomerProfile(ProfileViewModel profile)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Identitycard? identity = await _context.Identitycards.FindAsync(profile.IdentityId);
                    if (identity == null)
                    {
                        return new ResponseService { Success = false, Message = "Không tìm thấy CMND/CCCD" };
                    }
                    identity.Identitynumber = profile.IdentityCard;
                    identity.Backidentitycard = profile.BackIdentityCard;
                    identity.Frontidentitycard = profile.FrontIdentityCard;

                    Account? account = await _context.Accounts.FindAsync(profile.AccountId);
                    if (account == null)
                    {
                        return new ResponseService { Success = false, Message = "Không tìm thấy tài khoản" };
                    }
                    account.Email = profile.Email;
                    account.Phone = profile.Phone;
                    account.Avatar = profile.Avatar;
                    account.Lockenable = profile.LockStatus;

                    Customer? customer = await _context.Customers.FindAsync(profile.ProfileId);
                    if (customer == null)
                    {
                        return new ResponseService { Success = false, Message = "Không tìm thấy khách hàng" };
                    }
                    customer.Fullname = profile.FullName;
                    customer.Addressdetail = profile.AddressDetail;
                    customer.Gender = profile.Gender == "Nam" ? "M" : "F";
                    customer.Birth = profile.BirthDate;
                    customer.Districtid = profile.SelectedDistrictId;

                    _context.Identitycards.Update(identity);
                    _context.Accounts.Update(account);
                    _context.Customers.Update(customer);

                    int result = _context.SaveChanges();
                    transaction.Commit();
                    if (result > 0)
                    {
                        return new ResponseService { Success = true, Message = "Cập nhật thành công" };
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return new ResponseService { Success = false, Message = "Cập nhật không thành công" };
        }

        public async Task<ResponseService> UpdateTutorProfile(TutorProfileViewModel profile)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Identitycard? identity = await _context.Identitycards.FindAsync(profile.IdentityId);
                    if (identity == null)
                    {
                        return new ResponseService { Success = false, Message = "Không tìm thấy CMND/CCCD" };
                    }
                    identity.Identitynumber = profile.IdentityCard;
                    identity.Backidentitycard = profile.BackIdentityCard;
                    identity.Frontidentitycard = profile.FrontIdentityCard;

                    Account? account = await _context.Accounts.FindAsync(profile.AccountId);
                    if (account == null)
                    {
                        return new ResponseService { Success = false, Message = "Không tìm thấy tài khoản" };
                    }
                    account.Email = profile.Email;
                    account.Phone = profile.Phone;
                    account.Avatar = profile.Avatar;
                    account.Lockenable = profile.LockStatus;

                    Tutor? tutor = await _context.Tutors.FindAsync(profile.TutorId);
                    if (tutor == null)
                    {
                        return new ResponseService { Success = false, Message = "Không tìm thấy gia sư" };
                    }
                    tutor.Fullname = profile.Fullname;
                    tutor.Addressdetail = profile.AddressDetail;
                    tutor.Gender = profile.Gender == "Nam" ? "M" : "F";
                    tutor.Birth = profile.Birth;
                    tutor.Districtid = profile.SelectedDistrictId;
                    tutor.College = profile.College;
                    tutor.Area = profile.Area;
                    tutor.Additionalinfo = profile.Additionalinfo;
                    tutor.Academicyearfrom = profile.Academicyearfrom;
                    tutor.Academicyearto = profile.Academicyearto;
                    tutor.Typetutor = profile.TypeTutor;

                    _context.Identitycards.Update(identity);
                    _context.Accounts.Update(account);
                    _context.Tutors.Update(tutor);
                    int result = _context.SaveChanges();
                    transaction.Commit();
                    if (result > 0)
                    {
                        return new ResponseService { Success = true, Message = "Cập nhật thành công" };
                    }
                    else
                    {
                        return new ResponseService { Success = false, Message = "Cập nhật không thành công" };
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return new ResponseService { Success = false, Message = "Cập nhật không thành công" };
        }
    }
}