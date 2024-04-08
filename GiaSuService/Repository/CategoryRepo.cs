using GiaSuService.AppDbContext;
using GiaSuService.EntityModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.Repository
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly DvgsDbContext _context;
        public CategoryRepo(DvgsDbContext context)
        {
            _context = context;
        }


        public async Task<List<GradeViewModel>> GetAllGrades()
        {
            return await _context.Grades.AsNoTracking()
                .Select(p => new GradeViewModel
                {
                    GradeId = p.Id,
                    GradeName = p.Name,
                    Value = p.Value,
                })
                .OrderBy(p => p.Value)
                .ToListAsync();
        }

        public async Task<List<SessionViewModel>> GetAllSessions()
        {
            return await _context.SessionDates.AsNoTracking()
                .Select(p => new SessionViewModel
                {
                    SessionId = p.Id,
                    SessionName = p.Name,
                    Value = p.Value
                })
                .OrderBy(p => p.Value)
                .ToListAsync();
        }

        public async Task<List<SubjectViewModel>> GetAllSubjects()
        {
            return await _context.Subjects.AsNoTracking()
                .Select(p=> new SubjectViewModel
                {
                    SubjectId = p.Id,
                    SubjectName = p.Name,
                    Value = p.Value
                })
                .OrderBy(p => p.Value)
                .ToListAsync();
        }

        public async Task<List<TutorTypeViewModel>> GetAllTutorTypes()
        {
            return await _context.TutorTypes.AsNoTracking()
                .Select(p => new TutorTypeViewModel
                    {
                        TutorTypeId = p.Id,
                        TypeName = p.Name,
                        Value=p.Value,
                    })
                .OrderBy(p => p.Value)
                .ToListAsync();
        }

        public Task<GradeViewModel> GetGradeById(int gradeId)
        {
            throw new NotImplementedException();
        }

        public async Task<SessionViewModel?> GetSessionById(int sessionId)
        {
            return await _context.SessionDates.Select(p => new SessionViewModel { SessionId = p.Id, SessionName = p.Name, Value = p.Value })
                .FirstOrDefaultAsync(p => p.SessionId == sessionId);
        }

        public async Task<List<GradeViewModel>> GetSubGrades(List<int> ids)
        {
            return await _context.Grades.AsNoTracking()
                .Select(p => new GradeViewModel
                {
                    GradeId = p.Id,
                    GradeName = p.Name,
                    Value = p.Value,
                })
                .OrderBy(p => p.Value)
                .Where(p => ids.Contains(p.Value))
                .ToListAsync();
        }

        public Task<SubjectViewModel> GetSubjectById(int subjectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SessionViewModel>> GetSubSessions(List<int> ids)
        {
            return await _context.SessionDates.AsNoTracking()
                .Select(p => new SessionViewModel
                {
                    SessionId = p.Id,
                    SessionName = p.Name,
                    Value = p.Value
                })
                .Where(p => ids.Contains(p.SessionId))
                .OrderBy(p => p.Value)
                .ToListAsync();
        }

        public Task<List<SubjectViewModel>> GetSubSubjects(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<TutorTypeViewModel> GetTutorTypeById(int typeId)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> UpdateSessionDate(SessionViewModel session)
        {
            using(var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int count = await _context.SessionDates.AsNoTracking().CountAsync();
                    SessionDate? res = await _context.SessionDates.FindAsync(session.SessionId);
                    if(res == null)
                    {
                        return false;
                    }
                    int newValue = Math.Min(count, int.Max(1, session.Value));
                    int oldValue = res.Value;
                    res.Value = count + 1;
                    _context.SaveChanges();
                    if(oldValue < newValue)
                    {
                        var sessions = _context.SessionDates.Where(p => p.Value > oldValue).OrderBy(p => p.Value);
                        foreach(var ss in sessions)
                        {
                            ss.Value--;
                        }

                    }else if(oldValue > newValue)
                    {
                        var sessions = _context.SessionDates.Where(p => p.Value < oldValue).OrderBy(p => p.Value);
                        foreach (var ss in sessions)
                        {
                            ss.Value++;
                        }
                    }
                    res.Name = session.SessionName;
                    res.Value = newValue;
                    //res.Value = session.Value;
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    transaction.Commit();

                    return true;
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
        public async Task<bool> CreateSessionDate(SessionViewModel session)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int count = await _context.SessionDates.AsNoTracking().CountAsync();
                    int newValue = Math.Min(count, int.Max(1, session.Value));

                    var sessions = _context.SessionDates.Where(p => p.Value >= newValue).OrderBy(p => p.Value);
                    foreach (var ss in sessions)
                    {
                        ss.Value++;
                    }
                    await _context.SaveChangesAsync();

                    SessionDate newData = new SessionDate { Name = session.SessionName, Value = newValue };
                    _context.SessionDates.Add(newData);
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> IsUniqueName(string name, Type type)
        {
            if(type == typeof(SessionViewModel))
            {
                return (await _context.SessionDates.FirstOrDefaultAsync(p => p.Name == name)) == null;
            }
            else if (type == typeof(SubjectViewModel))
            {
                return (await _context.Subjects.FirstOrDefaultAsync(p => p.Name == name)) == null;
            }
            else if(type == typeof(GradeViewModel))
            {
                return (await _context.Grades.FirstOrDefaultAsync(p => p.Name == name)) == null;
            }

            return false;
        }

        public async Task<bool> DeleteSessionDate(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var session = await _context.SessionDates.FindAsync(id);
                    if(session == null)
                    {
                        return false;
                    }
                    var oldValue = session.Value;
                    _context.SessionDates.Remove(session);
                    await _context.SaveChangesAsync();

                    var sessions = _context.SessionDates.Where(p => p.Value > oldValue).OrderBy(p => p.Value);
                    foreach (var ss in sessions)
                    {
                        ss.Value--;
                    }
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}
