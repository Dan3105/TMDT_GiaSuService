﻿using GiaSuService.Configs;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Services.Interface
{
    public interface ITutorService
    {
        //if all id in param equal to 0 it will get all TutorProfiles
        public Task<List<AccountListViewModel>> GetTutorAccountsByFilter(
            int provinceId, int districtId, int subjectId, int gradeId, int page);

        public Task<PageCardListViewModel> GetTutorCardsByFilter(
            int provinceId, int districtId, int subjectId, int gradeId, int page);

        public Task<PageTutorRegisterListViewModel> GetRegisterTutoByStatus(int page, RegisterStatus status);
        public Task<TutorProfileViewModel?> GetTutorprofileById(int id);
        public Task<List<TutorCardViewModel>> GetSubTutors(List<int> ids);

        public Task<ResponseService> UpdateTutorProfileStatus(int tutorId, string status, string context);
        public Task<DifferenceUpdateRequestFormViewModel?> GetTutorUpdateRequest(int tutorId);

        public Task<IEnumerable<TutorProfileStatusDetailHistoryViewModel>> GetStatusTutorHistory(int tutorId);
        public Task<TutorProfileStatusDetailHistoryViewModel?> GetAStatusTutorHistory(int historyId);

        public Task<ResponseService> ApplyRequest(int tutorId, int requestId);
        public Task<ResponseService> CancelApplyRequest(int tutorId, int requestId);

        public Task<List<TutorApplyCardViewModel>> GetTutorApplyForm(int tutorId);

        public Task<RequestTutorApplyDetailViewModel?> GetRequestTutorApplyDetail(int requestId, int tutorId);
    }
}
