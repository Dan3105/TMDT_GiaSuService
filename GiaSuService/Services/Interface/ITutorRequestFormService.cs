﻿using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.CustomerViewModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorRequestFormService
    {
        public Task<TutorRequestProfileViewModel?> GetTutorRequestFormById(int formId);
        public Task<ResponseService> CreateForm(RequestTutorForm form, List<int> sessionList, List<int> tutorList);
        public Task<bool> UpdateForm(RequestTutorForm form, List<int> sessionList, string statusName);

        public Task<PageTutorRequestQueueListViewModel> GetTutorrequestQueue(AppConfig.FormStatus status, int page);
        public Task<PageTutorRequestListViewModel> GetTutorrequestCard(int provinceId, int districtId, int gradeId, int subjectId,
            AppConfig.FormStatus statusName, int page);
        public Task<PageTutorRequestListViewModel> GetTutorrequestCard(int provinceId, int districtId, int gradeId, int subjectId,
            AppConfig.FormStatus statusName, int page, int tutorId);


        public Task<ResponseService> UpdateStatusTutorRequest(int id, string status);
        public Task<ResponseService> UpdateStatusTutorApply(int tutorId, int requestId, string status);

        public Task<TutorRequestProfileEditViewModel?> GetTutorRequestProfileEdit(int id);
        public Task<ResponseService> UpdateTutorRequestProfileEdit(TutorRequestProfileEditViewModel model);


        public Task<List<CustomerTutorRequestViewModel>> GetCustomerTutorRequest(int accountId);
        public Task<List<TutorApplyRequestQueueViewModel>> GetTutorsApplyRequestQueue(int formId);

        public Task<TutorRequestCardViewModel?> GetTutorrequestDetail(int formId);

        public Task<ResponseService> UpdateTutorQueue(int requestId, int tutorId, string statusName, int employeeId);
    }
}
