using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Repository;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Services
{
    public class TutorService : ITutorService
    {
        private readonly DvgsDbContext _context;
        private readonly ITutorRepo _tutorRepository;
        private readonly IStatusRepo _statusRepository;
        private readonly IProfileRepo _profileRepository;
        private readonly IQueueRepo _queueRepository;
        public TutorService(DvgsDbContext context, ITutorRepo tutorRepository, IStatusRepo statusRepository, IProfileRepo profileRepository, IQueueRepo queueRepository)
        {
            _context = context;
            _tutorRepository = tutorRepository;
            _statusRepository = statusRepository;
            _profileRepository = profileRepository;
            _queueRepository = queueRepository;
        }
        public async Task<List<AccountListViewModel>> GetTutorAccountsByFilter(
            int subjectId, int districtId, int gradeId, int page)
        {
            List<AccountListViewModel> tutorprofiles = (await _tutorRepository.GetTutorAccountsByFilter(subjectId, districtId, gradeId, page)).ToList();
            return tutorprofiles;
        }

        public async Task<List<TutorCardViewModel>> GetTutorCardsByFilter(
           int subjectId, int districtId, int gradeId, int page)
        {
            IEnumerable<TutorCardViewModel> tutorprofiles = await _tutorRepository.GetTutorCardsByFilter(subjectId, districtId, gradeId, page);
            return tutorprofiles.ToList();
        }

        public async Task<List<TutorRegisterViewModel>> GetRegisterTutoByStatus(int page, RegisterStatus status)
        {
            List<TutorRegisterViewModel> tutorprofiles = await _tutorRepository.GetRegisterTutorOnPending(page, status);
            return tutorprofiles;
        }

        public async Task<TutorProfileViewModel?> GetTutorprofileById(int id)
        {
            string? current_status = await _statusRepository.GetLatestStatusInTutorRegister(id);
            if (current_status == null)
            {
                return null;
            }
            TutorProfileViewModel? tutorprofile = await _profileRepository.GetTutorProfile(id);
            if(tutorprofile == null)
            {
                return null;
            }

            tutorprofile.Formstatus = current_status;
            return tutorprofile;
        }

        public async Task<List<TutorCardViewModel>> GetSubTutors(List<int> ids)
        {
            return await _tutorRepository.GetSubTutorCardView(ids);
        }

        public async Task<ResponseService> UpdateTutorProfileStatus(int tutorId, string status, string context)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var dbStatus = await _statusRepository.GetStatus(status, register_status);
                    if (dbStatus == null)
                    {
                        return new ResponseService { Message = "Không cập nhật được trạng thái vui lòng làm lại ", Success = false };
                    }

                    Tutor? tutor = await _tutorRepository.GetTutor(tutorId);
                    if (tutor == null)
                    {
                        return new ResponseService { Message = "Không tìm được gia sư này ", Success = false };
                    }

                    bool isSuccess = false;
                    if (tutor.Status.Name.Equals(RegisterStatus.PENDING.ToString().ToLower()))
                    {
                        isSuccess = await _tutorRepository.UpdateTutor(tutor);
                    }
                    else if (tutor.Status.Name.Equals(RegisterStatus.UPDATE.ToString().ToLower()))
                    {
                        isSuccess = await _profileRepository.UpdateTutorProfileByUpdateForm(tutor);
                    }

                    tutor.Status = dbStatus;
                    tutor.TutorStatusDetails.Add(new TutorStatusDetail
                    {
                        Context = context,
                        Status = dbStatus,
                        CreateDate = DateTime.Now,
                    });

                    isSuccess = await _tutorRepository.UpdateTutor(tutor);
                    if (isSuccess)
                    {
                        transaction.Commit();
                        return new ResponseService { Success = true, Message = "Cập nhật thành công" };
                    }
                    else
                    {
                        transaction.Rollback();
                        return new ResponseService { Success = false, Message = "Lỗi hệ thống không cập nhật được" };
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return new ResponseService { Message = "Lỗi hệ thống", Success = false };
                }
            }
        }

        public async Task<DifferenceUpdateRequestFormViewModel?> GetTutorUpdateRequest(int tutorId)
        {
            var latestStatus = await _tutorRepository.GetLatestTutorStatusDetail(tutorId);
            if(latestStatus == null)
            {
                return null;
            }
            return await _profileRepository.GetTutorsDifferenceProfile(latestStatus.Id);
        }

        public async Task<IEnumerable<TutorProfileStatusDetailHistoryViewModel>> GetStatusTutorHistory(int tutorId)
        {
            return await _tutorRepository.GetTutorProfilesHistoryDetail(tutorId);
        }

        public async Task<TutorProfileStatusDetailHistoryViewModel?> GetAStatusTutorHistory(int historyId)
        {
            var historyDetail = await _tutorRepository.GetATutorProfileHistoryDetail(historyId);
            if(historyDetail == null)
            {
                return null;
            }
            try
            {
                var jsonContext = historyDetail.Context;
                TutorFormUpdateProfileViewModel? data = JsonConvert.DeserializeObject<TutorFormUpdateProfileViewModel>(jsonContext);

                if (data != null)
                {
                    

                    var tutorType = _context.TutorTypes.AsNoTracking();
                    var districts = _context.Districts.Include(p => p.Province).AsNoTracking();
                    var grades = _context.Grades.AsNoTracking().OrderBy(p => p.Value);
                    var sessions = _context.SessionDates.AsNoTracking().OrderBy(p => p.Value);
                    var subjects = _context.Subjects.AsNoTracking().OrderBy(p => p.Value);

                    if (data.SelectedDistrictId != 0)
                    {
                        var mdistrict = (await districts.FirstOrDefaultAsync(p => p.Id == data.SelectedDistrictId));
                        if (mdistrict == null) { throw new KeyNotFoundException(); }
                        string modifiedData = string.IsNullOrEmpty(data.AddressDetail) ? data.AddressDetail : data.AddressDetail;
                        data.FormatAddress = $"{mdistrict.Province.Name}, {mdistrict.Name}, {modifiedData}";
                    }

                    if (data.SelectedTutorTypeId != 0)
                    {
                        data.FormatTutorType = (await tutorType.FirstOrDefaultAsync(p => p.Id == data.SelectedTutorTypeId))?.Name ?? throw new NullReferenceException();
                    }
                    if (data.SelectedSessionIds.Any())
                    {
                        data.FormatSessions = string.Join(", ", sessions.Where(p =>
                                                                            data.SelectedSessionIds.Contains(p.Id)).Select(p => p.Name));
                    }
                    if (data.SelectedSubjectIds.Any())
                    {
                        data.FormatSubjects = string.Join(", ", subjects.Where(p =>
                                                                            data.SelectedSubjectIds.Contains(p.Id)).Select(p => p.Name));
                    }
                    if (data.SelectedGradeIds.Any())
                    {
                        data.FormatGrades = string.Join(", ", grades.Where(p =>
                                                                            data.SelectedGradeIds.Contains(p.Id)).Select(p => p.Name));
                    }
                    if (data.SelectedDistricts.Any())
                    {
                        data.FormatTeachingArea = string.Join(", ", districts.Where(p =>
                                                                            data.SelectedDistricts.Contains(p.Id)).Select(p => p.Name));

                    }
                    historyDetail.DetailModified = data;
                }

                return historyDetail;
            }

            catch (Exception) {
                return null;
            }
        }

        public async Task<ResponseService> ApplyRequest(int tutorId, int requestId)
        {
            var curr_status = await _statusRepository.GetLatestStatusInTutorRegister(tutorId);
            if (curr_status == null)
            {
                return new ResponseService { Success = false, Message = "Không lấy được trạng thái gia sư hiện tại" };
            }

            // Check if tutor status is approval then can apply, else cannot apply
            if (curr_status != RegisterStatus.APPROVAL.ToString().ToLower())
            {
                return new ResponseService { Success = false, Message = "Gia sư không có quyền đăng ký ứng tuyển. Vui lòng chờ nhân viên duyệt." };
            }

            Status? status = await _statusRepository.GetStatus(AppConfig.QueueStatus.PENDING.ToString(), AppConfig.queue_status.ToString());
            if(status == null)
            {
                return new ResponseService { Success = false, Message = "Không lấy được trạng thái" };
            }

            bool isSuccess = await _queueRepository.AddTutorToQueue(tutorId, requestId, status.Id);
            if (isSuccess)
            {
                return new ResponseService { Success = true, Message = "Ứng tuyển thành công" };
            }
            return new ResponseService { Success = false, Message = "Ứng tuyển thất bại" };
        }

        public async Task<ResponseService> CancelApplyRequest(int tutorId, int requestId)
        {
            Status? status = await _statusRepository.GetStatus(AppConfig.QueueStatus.CANCEL.ToString(), AppConfig.queue_status.ToString());
            if (status == null)
            {
                return new ResponseService { Success = false, Message = "Không lấy được trạng thái" };
            }

            bool isSuccess = await _queueRepository.CancelApplyRequest(tutorId, requestId, status.Id);
            if (isSuccess)
            {
                return new ResponseService { Success = true, Message = "Huỷ ứng tuyển thành công" };
            }
            return new ResponseService { Success = false, Message = "Huỷ ứng tuyển thất bại" };
        }

        public async Task<List<TutorApplyFormViewModel>> GetTutorApplyForm(int tutorId)
        {
            return await _tutorRepository.GetListTutorApplyForm(tutorId);
        }

        public async Task<RequestTutorApplyDetailViewModel?> GetTutorRequestProfileById(int requestId)
        {
            return await _tutorRepository.GetTutorRequestProfile(requestId);
        }
    }
}
