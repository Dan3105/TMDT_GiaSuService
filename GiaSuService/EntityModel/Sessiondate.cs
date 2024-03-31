using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Sessiondate
{
    public int Id { get; set; }

    public DateOnly Name { get; set; }

    public int Value { get; set; }

    public virtual ICollection<Tutorrequestform> Tutorrequests { get; set; } = new List<Tutorrequestform>();

    public virtual ICollection<Tutor> Tutors { get; set; } = new List<Tutor>();
}
