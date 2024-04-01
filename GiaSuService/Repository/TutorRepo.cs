using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Repository
{
    public class TutorRepo : ITutorRepo
    {
        private readonly DvgsDbContext _context;
        public TutorRepo(DvgsDbContext context)
        {
            _context = context;
        }
        public void Create(Tutor entity)
        {
            _context.Tutors.Add(entity);
        }

        public void Delete(Tutor entity)
        {
            _context.Tutors.Remove(entity);
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Update(Tutor entity)
        {
            _context.Tutors.Update(entity);
        }
        public async Task<IEnumerable<Tutor>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId)
        {
            var filteredTutors = await _context.Tutors
                .Include(p => p.Account)
                .Include(p => p.Subjects)
                .Include(p => p.Districts)
                .Include(p => p.Grades) 
            .Where(tp => (subjectId == 0 || tp.Subjects.Any(s => s.Id == subjectId))
                      && (districtId == 0 || tp.Districts.Any(d => d.Id == districtId))
                      && (gradeId == 0 || tp.Grades.Any(g => g.Id == gradeId))
                     )// && (tp.Formstatus == RegisterStatus.APPROVAL))
            .ToListAsync();

            return filteredTutors;
        }

        public async Task<IEnumerable<Tutor>> GetTutorprofilesByClassId(int classId)
        {
            //Get list tutorId by classId
            var tutorIds = await _context.Tutorqueues
                                .Where(p => p.Tutorrequestid == classId)
                                .Include(p => p.Tutor)
                                .Select(p => p.Tutorid)
                                .ToListAsync();

            // Get list Tutorprofile by tutorId
            var tutorProfiles = await _context.Tutors
                .Where(p => tutorIds.Contains(p.Id))
                .ToListAsync();

            return tutorProfiles;
        }

        //public async Task<IEnumerable<Tutor>> GetTutorprofilesByRegisterStatus(RegisterStatus status)
        //{
        //    // Get list Tutorprofile by tutorId
        //    var tutorProfiles = await _context.Tutorprofiles
        //        .Include(p => p.Account)
        //        .Where(p => p.Formstatus == status)
        //        .OrderBy(p => p.Account.Createdate)
        //        .ToListAsync();

        //    return tutorProfiles;
        //}

        public async Task<bool> UpdateProfile(Tutor tutor)
        {
            if(tutor != null)
            {
                _context.Tutors.Update(tutor);
                return await SaveChanges();
            }

            return false;
        }

        public async Task<bool> UpdateRegisterStatus(int tutorProfileId)
        {
            Tutor? tutorProfile = await _context.Tutors
                                    .FirstOrDefaultAsync(p => p.Id == tutorProfileId);
            if (tutorProfile != null)
            {
                //tutorProfile.Formstatus = status;
                return await UpdateProfile(tutorProfile);
            }

            return false;
        }

        public async Task<Tutor?> GetTutorprofile(int id)
        {
            Tutor? tutorProfile = await _context.Tutors
                                .Include (p => p.Account)
                               .FirstOrDefaultAsync(p=> p.Id == id);
            return tutorProfile;
        }

        public async Task<Tutor?> GetTutorprofileByAccountId(int accountId)
        {
            Tutor? tutorProfile = await _context.Tutors
                                .Include(p => p.Account)
                               .FirstOrDefaultAsync(p => p.Accountid == accountId);
            return tutorProfile;
        }

        public async Task<List<Tutor>> GetSubTutorProfile(List<int> ids)
        {
            var tutors = await _context.Tutors
                .Include(p => p.Account)
                .Where(p => ids.Contains(p.Id)).ToListAsync();
            return tutors;
        }
    }
}
