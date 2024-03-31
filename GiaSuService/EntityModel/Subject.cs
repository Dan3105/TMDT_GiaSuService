using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Value { get; set; }

    public virtual ICollection<Tutorrequestform> Tutorrequestforms { get; set; } = new List<Tutorrequestform>();

    public virtual ICollection<Tutor> Tutors { get; set; } = new List<Tutor>();
}
