using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using GiaSuService.Models.UtilityViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using GiaSuService.Models.TutorViewModel;
using Newtonsoft.Json;

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
                    IsActive = tutor.IsActive ?? false,

                    TutorType = new TutorTypeViewModel
                    {
                        TutorTypeId = tutor.TutorTypeId,
                        TypeName = tutor.TutorType.Name
                    },

                    IdentityCard = tutor.Identity.IdentityNumber,
                    FrontIdentityCard = tutor.Identity.FrontIdentityCard,
                    BackIdentityCard = tutor.Identity.BackIdentityCard,

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

        public async Task<bool> UpdateRequestTutorProfile(TutorFormUpdateProfileViewModel? modified)
        {
            if(modified == null)
            {
                return true;
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var exitsTutor = await GetTutorFormUpdateProfile(modified.TutorId);
                    if(exitsTutor == null)
                    {
                        return false;
                    }
                    var status = await _context.Statuses.Select(p => new {p.Id, p.Name}).
                        FirstOrDefaultAsync(p => p.Name.Equals(AppConfig.RegisterStatus.UPDATE.ToString().ToLower()));
                    if(status == null)
                    {
                        throw new NullReferenceException();
                    }

                    string jsonContext = JsonConvert.SerializeObject(modified);
                    TutorStatusDetail request = new TutorStatusDetail()
                    {
                        Context = jsonContext,
                        CreateDate = DateTime.Now,
                        TutorId = exitsTutor.TutorId,
                        StatusId = status.Id,
                    };

                    _context.TutorStatusDetails.Add(request);
                    await _context.SaveChangesAsync();

                    if(modified.IsActive != exitsTutor.IsActive)
                    {
                        _context.Tutors
                            .ExecuteUpdate(x => x 
                            .SetProperty(p => p.IsActive, modified.IsActive)
                            .SetProperty(p => p.StatusId, status.Id));

                        await _context.SaveChangesAsync();
                    }
                    _context.Tutors.ExecuteUpdate(x => x.SetProperty(p => p.StatusId, status.Id));
                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return false;
        }

        public async Task<TutorFormUpdateProfileViewModel?> GetTutorFormUpdateProfile(int tutorId)
        {
            return await _context.Tutors.AsNoTracking()
                .Select(p => new TutorFormUpdateProfileViewModel
                {
                    TutorId = p.Id,
                    Email = p.Account.Email,
                    Phone = p.Account.Phone,
                    Avatar = p.Account.Avatar,

                    IdentityCard = p.Identity.IdentityNumber,
                    FrontIdentityCard = p.Identity.FrontIdentityCard,
                    BackIdentityCard = p.Identity.BackIdentityCard,

                    Fullname = p.FullName,
                    Gender = p.Gender,
                    AddressDetail = p.AddressDetail,
                    SelectedDistrictId = p.DistrictId,
                    SelectedProvinceId = p.District.ProvinceId,
                    Birth = p.Birth,

                    Area = p.Area,
                    College = p.College,
                    Academicyearfrom = p.AcademicYearFrom,
                    Academicyearto = p.AcademicYearTo,
                    Additionalinfo = p.AdditionalInfo,
                    IsActive = p.IsActive ?? false,

                    SelectedTutorTypeId = p.TutorTypeId,
                    selectedDistricts = p.Districts.Select(p => p.Id),
                    selectedGradeIds = p.Grades.Select(p => p.Id),
                    selectedSessionIds = p.Sessions.Select(p => p.Id),
                    selectedSubjectIds = p.Subjects.Select(p => p.Id)
                })
                .FirstOrDefaultAsync(p => p.TutorId == tutorId);
        }


        #endregion
    }
}