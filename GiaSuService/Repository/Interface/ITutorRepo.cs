﻿using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Repository.Interface
{
    public interface ITutorRepo
    {
        //if param is an empty string it will get all TutorProfiles
        public Task<IEnumerable<AccountListViewModel>> GetTutorAccountsByFilter(
            int subjectId, int districtId, int gradeId, int page);
        public Task<IEnumerable<TutorCardViewModel>> GetTutorCardsByFilter(
            int subjectId, int districtId, int gradeId, int page);
        public Task<IEnumerable<Tutor>> GetTutorprofilesByClassId(int classId);
        public Task<List<TutorCardViewModel>> GetSubTutorCardView(List<int> ids);
        public Task<List<TutorRegisterViewModel>> GetRegisterTutorOnPending(int page, RegisterStatus status);

        public Task<Tutor?> GetTutor(int id);

//Only update status 
        public Task<bool> UpdateTutor(Tutor tutor);
        public Task<bool> SaveChanges();

    }
}
