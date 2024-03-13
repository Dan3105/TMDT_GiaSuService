using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Tutorprofile
{
    public int Id { get; set; }

    public string College { get; set; } = null!;

    public string Area { get; set; } = null!;

    public string Currentstatus { get; set; } = null!;

    public string? Additionalinfo { get; set; }

    public int Accountid { get; set; }

    public int Formid { get; set; }

    public short Academicyearfrom { get; set; }

    public short Academicyearto { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Classtutorqueue> Classtutorqueues { get; set; } = new List<Classtutorqueue>();

    public virtual Formregistertutor Form { get; set; } = null!;

    public virtual Formregistertutor? Formregistertutor { get; set; }

    public virtual ICollection<District> Districts { get; set; } = new List<District>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Sessiondate> Sessions { get; set; } = new List<Sessiondate>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
