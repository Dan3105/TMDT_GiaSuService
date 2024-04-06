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
                    .FirstOrDefaultAsync(p => p.AccountId == accountId))?.Id;

            }

            if (roleName.Equals(AppConfig.TUTORROLENAME.ToLower()))
            {
                return (await _context.Tutors.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.AccountId == accountId))?.Id;
            }

            if (roleName.Equals(AppConfig.CUSTOMERROLENAME))
            {
                return (await _context.Customers.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.AccountId == accountId))?.Id;
            }

            return null;
        }

        public async Task<IdentityCard?> GetIdentitycard(string identityNumber)
        {
            return await _context.IdentityCards.AsNoTracking().FirstOrDefaultAsync(x => x.IdentityNumber == identityNumber);
        }

        public async Task<List<AccountListViewModel>> GetEmployeeList(int crrPage)
        {
            IQueryable<AccountListViewModel> query = _context.Employees
                .AsNoTracking()
                .Where(p => p.Account.RoleId != 1)
                .Select(p => new AccountListViewModel()
                {
                    Email = p.Account.Email,
                    FullName = p.FullName,
                    Id = p.Id,
                    ImageUrl = p.Account.Avatar,
                    LockStatus = p.Account.LockEnable,
                })
                .OrderBy(p => p.Id)
                ;

            query = query.Skip(crrPage * AppConfig.ROWS_ACCOUNT_LIST)
                    .Take(AppConfig.ROWS_ACCOUNT_LIST);

            return await query.ToListAsync();
        }


        #region Get_Profile_View_Model_Employee_Customer_Tutor
        public async Task<ProfileViewModel?> GetProfile(int profileId, string role)
        {
            if (role == null || role == "") return null;

            ProfileViewModel? profile = null;
            if(role == AppConfig.CUSTOMERROLENAME)
            {
                profile = await _context.Customers
                .AsNoTracking()
                .Select(p => new ProfileViewModel()
                {
                    ProfileId = p.Id,
                    IdentityId = p.IdentityId,
                    AccountId = p.AccountId,
                    Avatar = p.Account.Avatar,
                    Email = p.Account.Email,
                    BirthDate = p.Birth,
                    FullName = p.FullName,
                    LockStatus = p.Account.LockEnable,
                    Phone = p.Account.Phone,
                    Gender = (p.Gender == "M" ? "Nam" : "Nữ"),
                    IdentityCard = p.Identity.IdentityNumber,
                    FrontIdentityCard = p.Identity.FrontIdentityCard,
                    BackIdentityCard = p.Identity.BackIdentityCard,

                    AddressDetail = p.AddressDetail,
                    SelectedDistrictId = p.DistrictId,
                    SelectedProvinceId = p.District.ProvinceId,
                })
                .FirstOrDefaultAsync(p => p.ProfileId == profileId);
            }
            else if (role == AppConfig.EMPLOYEEROLENAME || role == AppConfig.ADMINROLENAME)
            {
                profile = await _context.Employees
                .AsNoTracking()
                .Select(p => new ProfileViewModel()
                {
                    ProfileId = p.Id,
                    IdentityId = p.IdentityId,
                    AccountId = p.AccountId,
                    Avatar = p.Account.Avatar,
                    Email = p.Account.Email,
                    BirthDate = p.Birth,
                    FullName = p.FullName,
                    LockStatus = p.Account.LockEnable,
                    Phone = p.Account.Phone,
                    Gender = (p.Gender == "M" ? "Nam" : "Nữ"),
                    IdentityCard = p.Identity.IdentityNumber,
                    FrontIdentityCard = p.Identity.FrontIdentityCard,
                    BackIdentityCard = p.Identity.BackIdentityCard,

                    AddressDetail = p.AddressDetail,
                    SelectedDistrictId = p.DistrictId,
                    SelectedProvinceId = p.District.ProvinceId,
                })
                .FirstOrDefaultAsync(p => p.ProfileId == profileId);
            }

            return profile;
        }

        public async Task<TutorProfileViewModel?> GetTutorProfile(int tutorId)
        {
            var profile = await _context.Tutors
                .Select(tutor => new TutorProfileViewModel
                {
                    TutorId = tutor.Id,
                    AccountId = tutor.AccountId,
                    IdentityId = tutor.IdentityId,
                    Email = tutor.Account.Email,
                    Phone = tutor.Account.Phone,
                    LockEnable = tutor.Account.LockEnable,
                    Createdate = DateOnly.FromDateTime(tutor.Account.CreateDate),
                    Avatar = tutor.Account.Avatar,

                    Fullname = tutor.FullName,
                    Gender = tutor.Gender == "M" ? "Nam" : "Nữ",
                    AddressDetail = tutor.AddressDetail,
                    SelectedDistrictId = tutor.DistrictId,
                    SelectedProvinceId = tutor.District.ProvinceId,

                    Area = tutor.Area,
                    College = tutor.College,
                    Academicyearfrom = tutor.AcademicYearFrom,
                    Academicyearto = tutor.AcademicYearTo,
                    Additionalinfo = tutor.AdditionalInfo,
                    Birth = tutor.Birth,
                    IsActive = tutor.IsActive,

                    TutorType = tutor.TutorTypeId,

                    IdentityCard = tutor.Identity.IdentityNumber,
                    FrontIdentityCard = tutor.Identity.FrontIdentityCard,
                    BackIdentityCard = tutor.Identity.BackIdentityCard,

                    //IsValid = tutor.Isvalid
                })
                .FirstOrDefaultAsync(p => p.TutorId == tutorId);

            return profile;
        }
        #endregion

        #region Update_Profile_Employee_Customer_Tutor
        public async Task<bool> UpdateProfile(ProfileViewModel profile, string role)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    IdentityCard? identity = await _context.IdentityCards.FindAsync(profile.IdentityId);
                    if (identity == null)
                    {
                        return false;
                    }
                    identity.IdentityNumber = profile.IdentityCard;
                    identity.BackIdentityCard = profile.BackIdentityCard;
                    identity.FrontIdentityCard = profile.FrontIdentityCard;

                    Account? account = await _context.Accounts.FindAsync(profile.AccountId);
                    if (account == null)
                    {
                        return false;
                    }
                    account.Email = profile.Email;
                    account.Phone = profile.Phone;
                    account.Avatar = profile.Avatar;
                    account.LockEnable = profile.LockStatus;

                    _context.IdentityCards.Update(identity);
                    _context.Accounts.Update(account);

                    if (role == AppConfig.EMPLOYEEROLENAME || role == AppConfig.ADMINROLENAME)
                    {
                        Employee? employee = await _context.Employees.FindAsync(profile.ProfileId);
                        if (employee == null)
                        {
                            return false;
                        }
                        employee.FullName = profile.FullName;
                        employee.AddressDetail = profile.AddressDetail;
                        employee.Gender = profile.Gender == "Nam" ? "M" : "F";
                        employee.Birth = profile.BirthDate;
                        employee.DistrictId = profile.SelectedDistrictId;
                        _context.Employees.Update(employee);
                    }
                    else
                    {
                        Customer? customer = await _context.Customers.FindAsync(profile.ProfileId);
                        if (customer == null)
                        {
                            return false;
                        }
                        customer.FullName = profile.FullName;
                        customer.AddressDetail = profile.AddressDetail;
                        customer.Gender = profile.Gender == "Nam" ? "M" : "F";
                        customer.Birth = profile.BirthDate;
                        customer.DistrictId = profile.SelectedDistrictId;
                        _context.Customers.Update(customer);
                    }

                    int result = _context.SaveChanges();
                    transaction.Commit();
                    if (result > 0)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return false;
        }

        public async Task<bool> UpdateTutorProfile(TutorProfileViewModel profile)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    IdentityCard? identity = await _context.IdentityCards.FindAsync(profile.IdentityId);
                    if (identity == null)
                    {
                        return false;
                    }
                    identity.IdentityNumber = profile.IdentityCard;
                    identity.BackIdentityCard = profile.BackIdentityCard;
                    identity.FrontIdentityCard = profile.FrontIdentityCard;

                    Account? account = await _context.Accounts.FindAsync(profile.AccountId);
                    if (account == null)
                    {
                        return false;
                    }
                    account.Email = profile.Email;
                    account.Phone = profile.Phone;
                    account.Avatar = profile.Avatar;
                    account.LockEnable = profile.LockEnable;

                    Tutor? tutor = await _context.Tutors.FindAsync(profile.TutorId);
                    if (tutor == null)
                    {
                        return false;
                    }
                    tutor.FullName = profile.Fullname;
                    tutor.AddressDetail = profile.AddressDetail;
                    tutor.Gender = profile.Gender == "Nam" ? "M" : "F";
                    tutor.Birth = profile.Birth;
                    tutor.DistrictId = profile.SelectedDistrictId;
                    tutor.College = profile.College;
                    tutor.Area = profile.Area;
                    tutor.AdditionalInfo = profile.Additionalinfo;
                    tutor.AcademicYearFrom = profile.Academicyearfrom;
                    tutor.AcademicYearTo = profile.Academicyearto;
                    tutor.TutorTypeId = profile.TutorType;
                    tutor.IsActive = profile.IsActive;

                    _context.IdentityCards.Update(identity);
                    _context.Accounts.Update(account);
                    _context.Tutors.Update(tutor);
                    int result = _context.SaveChanges();
                    transaction.Commit();
                    if (result > 0)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return false;
        }

        #endregion
    
    }
}
