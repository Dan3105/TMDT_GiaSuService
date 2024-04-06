using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class TutorType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Value { get; set; }

    public virtual ICollection<Tutor> Tutors { get; set; } = new List<Tutor>();
}
