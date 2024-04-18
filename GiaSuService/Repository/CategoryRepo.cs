using GiaSuService.AppDbContext;
using GiaSuService.Configs;
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
                    Fee = p.Fee,
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

        public async Task<GradeViewModel?> GetGradeById(int gradeId)
        {
            return await _context.Grades.Select(p => new GradeViewModel { GradeId = p.Id, GradeName = p.Name, Value = p.Value, Fee=p.Fee })
                .FirstOrDefaultAsync(p => p.GradeId == gradeId);
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

        public async Task<SubjectViewModel?> GetSubjectById(int subjectId)
        {
            return await _context.Subjects.Select(p => new SubjectViewModel { SubjectId = p.Id, SubjectName = p.Name, Value = p.Value })
                .FirstOrDefaultAsync(p => p.SubjectId == subjectId);
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
                        var sessions = _context.SessionDates.Where(p => p.Value > oldValue && p.Value <= newValue).OrderBy(p => p.Value);
                        foreach(var ss in sessions)
                        {
                            ss.Value--;
                        }

                    }else if(oldValue > newValue)
                    {
                        var sessions = _context.SessionDates.Where(p => p.Value < oldValue && p.Value >= newValue).OrderBy(p => p.Value);
                        foreach (var ss in sessions)
                        {
                            ss.Value++;
                        }
                    }
                    res.Name = Utility.FormatToCamelCase(session.SessionName);
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

                    SessionDate newData = new SessionDate { Name = Utility.FormatToCamelCase(session.SessionName), Value = newValue };
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
        public async Task<bool> CreateSubject(SubjectViewModel subject)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int count = await _context.Subjects.AsNoTracking().CountAsync();
                    int newValue = Math.Min(count, int.Max(1, subject.Value));

                    var subjects = _context.Subjects.Where(p => p.Value >= newValue).OrderBy(p => p.Value);
                    foreach (var ss in subjects)
                    {
                        ss.Value++;
                    }
                    await _context.SaveChangesAsync();

                    Subject newData = new Subject { Name = Utility.FormatToCamelCase(subject.SubjectName), Value = newValue };
                    _context.Subjects.Add(newData);
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
        public async Task<bool> UpdateSubject(SubjectViewModel subject)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int count = await _context.Subjects.AsNoTracking().CountAsync();
                    Subject? res = await _context.Subjects.FindAsync(subject.SubjectId);
                    if (res == null)
                    {
                        return false;
                    }
                    int newValue = Math.Min(count, int.Max(1, subject.Value));
                    int oldValue = res.Value;
                    res.Value = count + 1;
                    _context.SaveChanges();
                    if (oldValue < newValue)
                    {
                        var sessions = _context.Subjects.Where(p => p.Value > oldValue && p.Value <= newValue).OrderBy(p => p.Value);
                        foreach (var ss in sessions)
                        {
                            ss.Value--;
                        }

                    }
                    else if (oldValue > newValue)
                    {
                        var sessions = _context.Subjects.Where(p => p.Value < oldValue && p.Value >= newValue).OrderBy(p => p.Value);
                        foreach (var ss in sessions)
                        {
                            ss.Value++;
                        }
                    }
                    res.Name = Utility.FormatToCamelCase(subject.SubjectName);
                    res.Value = newValue;
                    //res.Value = session.Value;
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
        public async Task<bool> DeleteSubject(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var subject = await _context.Subjects.FindAsync(id);
                    if (subject == null)
                    {
                        return false;
                    }
                    var oldValue = subject.Value;
                    _context.Subjects.Remove(subject);
                    await _context.SaveChangesAsync();

                    var sessions = _context.Subjects.Where(p => p.Value > oldValue).OrderBy(p => p.Value);
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

        public async Task<bool> CreateGrade(GradeViewModel grade)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int count = await _context.Grades.AsNoTracking().CountAsync();
                    int newValue = Math.Min(count, int.Max(1, grade.Value));

                    var grades = _context.Grades.Where(p => p.Value >= newValue).OrderBy(p => p.Value);
                    foreach (var ss in grades)
                    {
                        ss.Value++;
                    }
                    await _context.SaveChangesAsync();

                    Grade newData = new Grade { Name = Utility.FormatToCamelCase(grade.GradeName), Value = newValue, Fee = grade.Fee };
                    _context.Grades.Add(newData);
                    await _context.SaveChangesAsync();

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
        public async Task<bool> UpdateGrade(GradeViewModel grade)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int count = await _context.Grades.AsNoTracking().CountAsync();
                    Grade? res = await _context.Grades.FindAsync(grade.GradeId);
                    if (res == null)
                    {
                        return false;
                    }
                    int newValue = Math.Min(count, int.Max(1, grade.Value));
                    int oldValue = res.Value;
                    res.Value = count + 1;
                    _context.SaveChanges();
                    if (oldValue < newValue)
                    {
                        var grades = _context.Grades.Where(p => p.Value > oldValue && p.Value <= newValue).OrderBy(p => p.Value);
                        foreach (var ss in grades)
                        {
                            ss.Value--;
                        }

                    }
                    else if (oldValue > newValue)
                    {
                        var grades = _context.Grades.Where(p => p.Value < oldValue && p.Value >= newValue).OrderBy(p => p.Value);
                        foreach (var ss in grades)
                        {
                            ss.Value++;
                        }
                    }
                    res.Name = Utility.FormatToCamelCase(grade.GradeName);
                    res.Value = newValue;
                    res.Fee = grade.Fee;

                    await _context.SaveChangesAsync();

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
        public async Task<bool> DeleteGrade(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var grade = await _context.Grades.FindAsync(id);
                    if (grade == null)
                    {
                        return false;
                    }
                    var oldValue = grade.Value;
                    _context.Grades.Remove(grade);
                    await _context.SaveChangesAsync();

                    var grades = _context.Grades.Where(p => p.Value > oldValue).OrderBy(p => p.Value);
                    foreach (var ss in grades)
                    {
                        ss.Value--;
                    }
                    await _context.SaveChangesAsync();

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
