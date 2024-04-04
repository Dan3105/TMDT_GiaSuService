﻿using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.CustomerViewModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.TutorViewModel;

namespace GiaSuService.Services.Interface
{
    public interface ITutorRequestFormService
    {
        public Task<TutorRequestProfileViewModel?> GetTutorRequestFormById(int formId);
        public Task<ResponseService> CreateForm(Tutorrequestform form, List<int> sessionList, List<int> tutorList);
        public Task<bool> UpdateForm(Tutorrequestform form, List<int> sessionList, string statusName);

        public Task<List<TutorRequestQueueViewModel>> GetTutorrequestQueue(AppConfig.FormStatus status, int page);
        public Task<List<TutorRequestCardViewModel>> GetTutorrequestCard(int districtId, int gradeId, int subjectId,
            AppConfig.FormStatus statusName, int page);
        public Task<ResponseService> UpdateStatusTutorRequest(int id, string status);

        public Task<TutorRequestProfileEditViewModel?> GetTutorRequestProfileEdit(int id);
    }
}
