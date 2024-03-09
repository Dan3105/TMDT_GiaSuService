using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Subject
{
    public int Id { get; set; }

    public string Subjectname { get; set; } = null!;

    public virtual ICollection<Classprofile> Classprofiles { get; set; } = new List<Classprofile>();

    public virtual ICollection<Tutorprofile> Tutors { get; set; } = new List<Tutorprofile>();
}
