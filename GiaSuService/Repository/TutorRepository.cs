using GiaSuService.AppDbContext;
using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using static GiaSuService.Configs.AppConfig;

namespace GiaSuService.Repository
{
    public class TutorRepository : ITutorRepository
    {
        private readonly TmdtDvgsContext _context;
        public TutorRepository(TmdtDvgsContext context)
        {
            _context = context;
        }
        public void Create(Tutorprofile entity)
        {
            _context.Tutorprofiles.Add(entity);
        }

        public void Delete(Tutorprofile entity)
        {
            _context.Tutorprofiles.Remove(entity);
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Update(Tutorprofile entity)
        {
            _context.Tutorprofiles.Update(entity);
        }
        public async Task<IEnumerable<Tutorprofile>> GetTutorprofilesByFilter(
            int subjectId, int districtId, int gradeId)
        {
            var filteredTutors = await _context.Tutorprofiles
                .Include(p => p.Account)
                .Include(p => p.Subjects)
                .Include(p => p.Districts)
                .Include(p => p.Grades) 
            .Where(tp => (subjectId == 0 || tp.Subjects.Any(s => s.Id == subjectId))
                      && (districtId == 0 || tp.Districts.Any(d => d.Id == districtId))
                      && (gradeId == 0 || tp.Grades.Any(g => g.Id == gradeId))
                      && (tp.Formstatus == RegisterStatus.APPROVAL))
            .ToListAsync();

            return filteredTutors;
        }

        public async Task<IEnumerable<Tutorprofile>> GetTutorprofilesByClassId(int classId)
        {
            //Get list tutorId by classId
            var tutorIds = await _context.Tutormatchrequestqueues
                                .Where(p => p.Formid == classId)
                                .Include(p => p.Tutor)
                                .Select(p => p.Tutorid)
                                .ToListAsync();

            // Get list Tutorprofile by tutorId
            var tutorProfiles = await _context.Tutorprofiles
                .Where(p => tutorIds.Contains(p.Id))
                .ToListAsync();

            return tutorProfiles;
        }

        public async Task<IEnumerable<Tutorprofile>> GetTutorprofilesByRegisterStatus(RegisterStatus status)
        {
            // Get list Tutorprofile by tutorId
            var tutorProfiles = await _context.Tutorprofiles
                .Include(p => p.Account)
                .Where(p => p.Formstatus == status)
                .OrderBy(p => p.Account.Createdate)
                .ToListAsync();

            return tutorProfiles;
        }

        public async Task<bool> UpdateProfile(Tutorprofile tutor)
        {
            if(tutor != null)
            {
                _context.Tutorprofiles.Update(tutor);
                return await SaveChanges();
            }

            return false;
        }

        public async Task<bool> UpdateRegisterStatus(int tutorProfileId, AppConfig.RegisterStatus status)
        {
            Tutorprofile? tutorProfile = await _context.Tutorprofiles
                                    .FirstOrDefaultAsync(p => p.Id == tutorProfileId);
            if (tutorProfile != null)
            {
                tutorProfile.Formstatus = status;
                return await UpdateProfile(tutorProfile);
            }

            return false;
        }

        public async Task<Tutorprofile?> GetTutorprofile(int id)
        {
            Tutorprofile? tutorProfile = await _context.Tutorprofiles
                                .Include (p => p.Account)
                               .FirstOrDefaultAsync(p=> p.Id == id);
            return tutorProfile;
        }

        public async Task<Tutorprofile?> GetTutorprofileByAccountId(int accountId)
        {
            Tutorprofile? tutorProfile = await _context.Tutorprofiles
                                .Include(p => p.Account)
                               .FirstOrDefaultAsync(p => p.Accountid == accountId);
            return tutorProfile;
        }

        public async Task<List<Tutorprofile>> GetSubTutorProfile(List<int> ids)
        {
            var tutors = await _context.Tutorprofiles
                .Include(p => p.Account)
                .Where(p => ids.Contains(p.Id)).ToListAsync();
            return tutors;
        }
    }
}
