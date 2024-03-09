using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Grade
{
    public int Id { get; set; }

    public string Gradename { get; set; } = null!;

    public virtual ICollection<Classprofile> Classprofiles { get; set; } = new List<Classprofile>();

    public virtual ICollection<Tutorprofile> Tutors { get; set; } = new List<Tutorprofile>();
}
