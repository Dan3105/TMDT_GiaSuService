using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Models.TutorViewModel;
using Newtonsoft.Json;
using GiaSuService.Models.EmployeeViewModel;

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
                    _context.Tutors
                        .Where(x => x.Id == exitsTutor.TutorId)
                        .ExecuteUpdate(x => x.SetProperty(p => p.StatusId, status.Id));

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
            var result = await _context.Tutors.AsNoTracking()
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
                    FormatTutorType = p.TutorType.Name,
                    SelectedDistricts = p.Districts.Select(p => p.Id),
                    SelectedGradeIds = p.Grades.Select(p => p.Id),
                    SelectedSessionIds = p.Sessions.Select(p => p.Id),
                    SelectedSubjectIds = p.Subjects.Select(p => p.Id),

                })
                .FirstOrDefaultAsync(p => p.TutorId == tutorId);

            return result;
        }

        public async Task<DifferenceUpdateRequestFormViewModel?> GetTutorsDifferenceProfile(int tutorId)
        {
            var currentTutorUser = await GetTutorFormUpdateProfile(tutorId);
            if(currentTutorUser == null)
            {
                return null;
            }

            var getLatestStatus = await _context.TutorStatusDetails
                .Include(p => p.Status)
                .AsNoTracking()
                .OrderByDescending(p => p.CreateDate)
                .FirstOrDefaultAsync(p => p.TutorId == tutorId);
            
            if(getLatestStatus == null || getLatestStatus.Status.Name.ToLower() != AppConfig.RegisterStatus.UPDATE.ToString().ToLower())
            {
                return null;
            }

            try
            {
                var jsonContext = getLatestStatus.Context;
                TutorFormUpdateProfileViewModel? data = JsonConvert.DeserializeObject<TutorFormUpdateProfileViewModel>(jsonContext);

                if(data == null)
                {
                    return null;
                }

                var tutorType = _context.TutorTypes.AsNoTracking();
                var districts = _context.Districts.Include(p => p.Province).AsNoTracking();
                var grades = _context.Grades.AsNoTracking().OrderBy(p => p.Value);
                var sessions = _context.SessionDates.AsNoTracking().OrderBy(p => p.Value);
                var subjects = _context.Subjects.AsNoTracking().OrderBy(p => p.Value);

                DifferenceUpdateRequestFormViewModel result = new DifferenceUpdateRequestFormViewModel();
                #region original handler
                result.Original = currentTutorUser;
                var district = (await districts.FirstOrDefaultAsync(p => p.Id == result.Original.SelectedDistrictId));
                if(district == null) { throw new KeyNotFoundException(); }
                result.Original.FormatAddress = $"{district.Province.Name}, {district.Name}, {currentTutorUser.AddressDetail}";
                result.Original.FormatTutorType = (await tutorType.FirstOrDefaultAsync(p => p.Id == currentTutorUser.SelectedTutorTypeId))?.Name ?? throw new NullReferenceException();
                result.Original.FormatSessions = string.Join(", ", sessions.Where(p => 
                                                                    currentTutorUser.SelectedSessionIds.Contains(p.Id)).Select(p => p.Name));
                result.Original.FormatSubjects = string.Join(", ", subjects.Where(p =>
                                                                    currentTutorUser.SelectedSubjectIds.Contains(p.Id)).Select(p => p.Name));
                result.Original.FormatGrades = string.Join(", ", grades.Where(p =>
                                                                    currentTutorUser.SelectedGradeIds.Contains(p.Id)).Select(p => p.Name));
                result.Original.FormatTeachingArea = string.Join(", ", districts.Where(p =>
                                                                    currentTutorUser.SelectedDistricts.Contains(p.Id)).Select(p => p.Name));
                #endregion

                #region modified handler
                result.Modified = data;
                if (data.SelectedDistrictId != 0)
                {
                    var mdistrict = (await districts.FirstOrDefaultAsync(p => p.Id == data.SelectedDistrictId));
                    if(mdistrict == null) { throw new KeyNotFoundException(); }
                    string modifiedData = string.IsNullOrEmpty(data.AddressDetail) ? currentTutorUser.AddressDetail : data.AddressDetail;
                    result.Modified.FormatAddress = $"{mdistrict.Province.Name}, {mdistrict.Name}, {modifiedData }";
                }

                if(data.SelectedTutorTypeId != 0)
                {
                    result.Modified.FormatTutorType = (await tutorType.FirstOrDefaultAsync(p => p.Id == data.SelectedTutorTypeId))?.Name ?? throw new NullReferenceException();
                }
                if(data.SelectedSessionIds.Any())
                {
                    result.Modified.FormatSessions = string.Join(", ", sessions.Where(p =>
                                                                        data.SelectedSessionIds.Contains(p.Id)).Select(p => p.Name));
                }
                if(data.SelectedSubjectIds.Any())
                {
                    result.Modified.FormatSubjects = string.Join(", ", subjects.Where(p =>
                                                                        data.SelectedSubjectIds.Contains(p.Id)).Select(p => p.Name));
                }
                if(data.SelectedGradeIds.Any())
                {
                    result.Modified.FormatGrades = string.Join(", ", grades.Where(p =>
                                                                        data.SelectedGradeIds.Contains(p.Id)).Select(p => p.Name));
                }
                if(data.SelectedDistricts.Any())
                {
                    result.Modified.FormatTeachingArea = string.Join(", ", districts.Where(p =>
                                                                        data.SelectedDistricts.Contains(p.Id)).Select(p => p.Name));

                }
                #endregion

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateTutorProfileByUpdateForm(Tutor original)
        {
            var getLatestStatus = await _context.TutorStatusDetails
                                .Include(p => p.Status)
                                .AsNoTracking()
                                .OrderByDescending(p => p.CreateDate)
                                .FirstOrDefaultAsync(p => p.TutorId == original.Id);
            if (getLatestStatus == null || getLatestStatus.Status.Name.ToLower() != AppConfig.RegisterStatus.UPDATE.ToString().ToLower())
            {
                return false;
            }

            try
            {
                var jsonContext = getLatestStatus.Context;
                TutorFormUpdateProfileViewModel? modified = JsonConvert.DeserializeObject<TutorFormUpdateProfileViewModel>(jsonContext);

                if (modified == null)
                {
                    return false;
                }

                await UpdateProperties(original, modified);
                _context.Tutors.Update(original);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task UpdateProperties(Tutor original, TutorFormUpdateProfileViewModel modified)
        {
            if (!string.IsNullOrEmpty(modified.Fullname))
                original.FullName = modified.Fullname;

            if (modified.Birth.HasValue)
                original.Birth = modified.Birth.Value;

            if (!string.IsNullOrEmpty(modified.Email))
                original.Account.Email = modified.Email;

            if (!string.IsNullOrEmpty(modified.Phone))
                original.Account.Phone = modified.Phone;

            if (!string.IsNullOrEmpty(modified.Gender))
                original.Gender = modified.Gender;

            if (!string.IsNullOrEmpty(modified.Avatar))
                original.Account.Avatar = modified.Avatar;

            if (modified.SelectedDistrictId != 0)
                original.DistrictId = modified.SelectedDistrictId;

            if (!string.IsNullOrEmpty(modified.AddressDetail))
                original.AddressDetail = modified.AddressDetail;

            if (!string.IsNullOrEmpty(modified.IdentityCard))
                original.Identity.IdentityNumber = modified.IdentityCard;

            if (!string.IsNullOrEmpty(modified.FrontIdentityCard))
                original.Identity.FrontIdentityCard = modified.FrontIdentityCard;

            if (!string.IsNullOrEmpty(modified.BackIdentityCard))
                original.Identity.BackIdentityCard = modified.BackIdentityCard;

            if (!string.IsNullOrEmpty(modified.College))
                original.College = modified.College;

            if (!string.IsNullOrEmpty(modified.Area))
                original.Area = modified.Area;

            if (modified.Additionalinfo != null)
                original.AdditionalInfo = modified.Additionalinfo;

            if (modified.Academicyearfrom != 0)
                original.AcademicYearFrom = modified.Academicyearfrom;

            if (modified.Academicyearto != 0)
                original.AcademicYearTo = modified.Academicyearto;

            if (modified.SelectedTutorTypeId != 0)
                original.TutorTypeId = modified.SelectedTutorTypeId;

            if (modified.SelectedSubjectIds != null && modified.SelectedSubjectIds.Any())
            {
                _context.Entry(original).Collection(p => p.Subjects).Load();
                //original.Subjects = modified.SelectedSubjectIds.Select(id => new Subject { Id = id }).ToList();
                var IdsToRemove = original.Subjects.Where(p => !modified.SelectedSubjectIds.Contains(p.Id)).ToList();
                foreach(var id in IdsToRemove)
                {
                    original.Subjects.Remove(id);
                }

                var IdsToAdd = modified.SelectedSubjectIds.Where(id => !original.Subjects.Any(s => s.Id == id)).ToList();
                foreach(var id in IdsToAdd)
                {
                    var dataById = await _context.Subjects.FindAsync(id);
                    if(dataById != null)
                    {
                        original.Subjects.Add(dataById);
                    }
                }
            }

            if (modified.SelectedSessionIds != null && modified.SelectedSessionIds.Any())
            {
                _context.Entry(original).Collection(p => p.Sessions).Load();
                var IdsToRemove = original.Sessions.Where(p => !modified.SelectedSessionIds.Contains(p.Id)).ToList();
                foreach (var id in IdsToRemove)
                {
                    original.Sessions.Remove(id);
                }

                var IdsToAdd = modified.SelectedSessionIds.Where(id => !original.Sessions.Any(s => s.Id == id)).ToList();
                foreach (var id in IdsToAdd)
                {
                    var dataById = await _context.SessionDates.FindAsync(id);
                    if (dataById != null)
                    {
                        original.Sessions.Add(dataById);
                    }
                }
            }

            if (modified.SelectedDistricts != null && modified.SelectedDistricts.Any())
            {
                _context.Entry(original).Collection(p => p.Districts).Load();
                var IdsToRemove = original.Districts.Where(p => !modified.SelectedDistricts.Contains(p.Id)).ToList();
                foreach (var id in IdsToRemove)
                {
                    original.Districts.Remove(id);
                }

                var IdsToAdd = modified.SelectedDistricts.Where(id => !original.Districts.Any(s => s.Id == id)).ToList();
                foreach (var id in IdsToAdd)
                {
                    var dataById = await _context.Districts.FindAsync(id);
                    if (dataById != null)
                    {
                        original.Districts.Add(dataById);
                    }
                }
            }

            if (modified.SelectedGradeIds != null && modified.SelectedGradeIds.Any())
            {
                _context.Entry(original).Collection(p => p.Grades).Load();
                var IdsToRemove = original.Grades.Where(p => !modified.SelectedGradeIds.Contains(p.Id)).ToList();
                foreach (var id in IdsToRemove)
                {
                    original.Grades.Remove(id);
                }

                var IdsToAdd = modified.SelectedGradeIds.Where(id => !original.Grades.Any(s => s.Id == id)).ToList();
                foreach (var id in IdsToAdd)
                {
                    var dataById = await _context.Grades.FindAsync(id);
                    if (dataById != null)
                    {
                        original.Grades.Add(dataById);
                    }
                }
            }
        }



        #endregion
    }
}