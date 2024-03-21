using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Subject
{
    public int Id { get; set; }

    public string Subjectname { get; set; } = null!;

    public short Value { get; set; }

    public virtual ICollection<Tutorrequestform> Tutorrequestforms { get; set; } = new List<Tutorrequestform>();

    public virtual ICollection<Tutorprofile> Tutors { get; set; } = new List<Tutorprofile>();
}
