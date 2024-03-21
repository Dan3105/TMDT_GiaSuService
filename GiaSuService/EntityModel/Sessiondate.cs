using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Sessiondate
{
    public int Id { get; set; }

    public string Sessiondate1 { get; set; } = null!;

    public short Value { get; set; }

    public virtual ICollection<Tutorprofile> Tutors { get; set; } = new List<Tutorprofile>();
}
