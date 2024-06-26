﻿using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.CustomerViewModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;

namespace GiaSuService.Services
{
    public class TutorRequestFormService : ITutorRequestFormService
    {
        private readonly DvgsDbContext _context;
        private readonly ITutorRequestRepo _tutorRequestRepo;
        private readonly IStatusRepo _statusRepo;
        private readonly IQueueRepo _queueRepo;

        public TutorRequestFormService(DvgsDbContext context, ITutorRequestRepo tutorRequestRepo, IStatusRepo statusRepo
            , IQueueRepo queueRepo)
        {
            _context = context;
            _tutorRequestRepo = tutorRequestRepo;
            _statusRepo = statusRepo;
            _queueRepo = queueRepo;
        }

        public async Task<TutorRequestProfileViewModel?> GetTutorRequestFormById(int formId)
        {
            TutorRequestProfileViewModel? form = (await _tutorRequestRepo.GetTutorRequestProfile(formId));
            if (form != null)
            {
                form.TutorCards = await _queueRepo.GetTutorInQueueByForm(form.FormId);
            }
            return form;
        }

        public async Task<ResponseService> CreateForm(RequestTutorForm form, List<int> listSession, List<int> listTutor)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Status? status = await _statusRepo.GetStatus(AppConfig.FormStatus.PENDING.ToString(), AppConfig.form_status);
                    if (status == null)
                    {
                        throw new NullReferenceException();
                    }
                    form.Status = status;
                    _tutorRequestRepo.Create(form);
                    await _tutorRequestRepo.SaveChanges();

                    if (!listSession.Any())
                    {
                        return new ResponseService { Success = false, Message = "Bạn chưa chọn ngày dạy" };
                    }
                    var sessionQueries = _context.SessionDates.Where(p => listSession.Contains(p.Id));
                    foreach(var session in sessionQueries)
                    {
                        form.Sessions.Add(session);
                    }
                    if (listTutor.Count > 0)
                    {
                        int? statusId = (await _statusRepo.GetStatus(AppConfig.QueueStatus.PENDING.ToString(), AppConfig.queue_status))?.Id;
                        if (statusId == null)
                        {
                            return new ResponseService { Success = false, Message = "Việc chọn gia sư không có lỗi vui lòng làm lại" };
                        }
                        bool isSuccess = _queueRepo.AddTutorsToQueue(form, listTutor, (int)statusId);
                        await _tutorRequestRepo.SaveChanges();
                    }
                    
                    
                    _tutorRequestRepo.Update(form);
                    await _tutorRequestRepo.SaveChanges();
                    transaction.Commit();
                    return new ResponseService { Success = true, Message = "Tạo đơn yêu cầu thành công" };
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return new ResponseService { Success = false, Message = "Lỗi hệ thống" };
                }
            }
           
        }

        public async Task<bool> UpdateForm(RequestTutorForm form, List<int> sessionList, string statusName)
        {
            try
            {
                Status? status = await _statusRepo.GetStatus(statusName, AppConfig.form_status);
                if (status == null)
                {
                    throw new NullReferenceException();
                }
                form.Status = status;
                _tutorRequestRepo.Update(form);
                var isSucced = await _tutorRequestRepo.SaveChanges();
                return isSucced;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<PageTutorRequestQueueListViewModel> GetTutorrequestQueue(AppConfig.FormStatus statusName, int page)
        {
            var status = await _statusRepo.GetStatus(statusName.ToString(), AppConfig.form_status);
            if(status == null)
            {
                return null!;
            }

            return await _tutorRequestRepo.GetTutorRequestQueueByStatus(status.Id, page);
        }

        [AllowAnonymous]
        public async Task<PageTutorRequestListViewModel> GetTutorrequestCard(int provinceId, int districtId, int gradeId, int subjectId,
            AppConfig.FormStatus statusName, int page)
        {
            var status = await _statusRepo.GetStatus(statusName.ToString(), AppConfig.form_status);
            if(status == null)
            {
                return null!;
            }
            return await _tutorRequestRepo.GetTutorRequestCardByStatus(provinceId, districtId, subjectId, gradeId, status.Id, page);
        }

        public async Task<PageTutorRequestListViewModel> GetTutorrequestCard(int provinceId, int districtId, int gradeId, int subjectId,
            AppConfig.FormStatus statusName, int page, int tutorId)
        {
            var status = await _statusRepo.GetStatus(statusName.ToString(), AppConfig.form_status);
            if (status == null)
            {
                return null!;
            }
            if(tutorId == 0)
            {
                return await _tutorRequestRepo.GetTutorRequestCardByStatus(provinceId, districtId, subjectId, gradeId, status.Id, page);
            }
            return await _tutorRequestRepo.GetTutorRequestCardByStatus(provinceId, districtId, subjectId, gradeId, status.Id, page, tutorId);
        }

        public async Task<ResponseService> UpdateStatusTutorRequest(int id, string status)
        {
            var dbStatus = await _statusRepo.GetStatus(status, AppConfig.form_status);
            if (dbStatus == null)
            {
                return new ResponseService { Message = "Không cập nhật được trạng thái vui lòng làm lại ", Success = false };
            }

            var tutorRequest = await _tutorRequestRepo.Get(id);
            if (tutorRequest == null)
            {
                return new ResponseService { Message = "Không tìm được đơn này trong hệ thống", Success = false };
            }

            tutorRequest.Status = dbStatus;
            _tutorRequestRepo.Update(tutorRequest);
            bool isSuccess = await _tutorRequestRepo.SaveChanges();
            if (isSuccess)
            {
                return new ResponseService { Success = true, Message = "Cập nhật thành công" };
            }
            else
            {
                return new ResponseService { Success = false, Message = "Lỗi hệ thống không cập nhật được" };
            }
        }

        public async Task<ResponseService> UpdateStatusTutorApply(int tutorId, int requestId, string status)
        {
            var dbStatus = await _statusRepo.GetStatus(status, AppConfig.queue_status);
            if (dbStatus == null)
            {
                return new ResponseService { Message = "Không cập nhật được trạng thái vui lòng làm lại ", Success = false };
            }

            bool isSuccess = await _tutorRequestRepo.UpdateTutorApplyStatus(tutorId, requestId, dbStatus);
            if (isSuccess)
            {
                return new ResponseService { Success = true, Message = "Cập nhật thành công" };
            }
            else
            {
                return new ResponseService { Success = false, Message = "Lỗi hệ thống không cập nhật được" };
            }
        }

        public async Task<TutorRequestProfileEditViewModel?> GetTutorRequestProfileEdit(int id)
        {
            return await _tutorRequestRepo.GetTutorRequestProfileEdit(id);
        }

        public async Task<List<CustomerTutorRequestViewModel>> GetCustomerTutorRequest(int customerId)
        {
            return await _tutorRequestRepo.GetListTutorRequestOfCustomer(customerId);
        }


        public async Task<ResponseService> UpdateTutorRequestProfileEdit(TutorRequestProfileEditViewModel model)
        {
            bool isSuccess = await _tutorRequestRepo.UpdateTutorRequestProfileEdit(model);
            if (isSuccess)
            {
                return new ResponseService { Success = true, Message = "Cập nhật thành công" };
            }

            return new ResponseService { Success = false, Message = "Cập nhật thất bại" };
        }

        public async Task<List<TutorApplyRequestQueueViewModel>> GetTutorsApplyRequestQueue(int formId)
        {
            return await _queueRepo.GetTutorsApplyRequestQueue(formId);
        }

        public async Task<TutorRequestCardViewModel?> GetTutorrequestDetail(int formId)
        {
            return await _tutorRequestRepo.GetTutorRequestCardById(formId);
        }

        public async Task<ResponseService> UpdateTutorQueue(int requestId, int tutorId, string statusName, int employeeId)
        {
            Status? status = await _statusRepo.GetStatus(statusName, AppConfig.queue_status);
            if (status == null)
            {
                return new ResponseService { Message = "Không tìm được trạng thái này trong hệ thống", Success = false };
            }


            bool isSuccess = await _queueRepo.UpdateTutorQueue(requestId, tutorId, status, employeeId);
            if (isSuccess)
            {
                return new ResponseService { Success = true, Message = "Cập nhật trạng thái thành công" };
            }

            return new ResponseService { Success = false, Message = "Lỗi hệ thống" };
        }
    }
}
