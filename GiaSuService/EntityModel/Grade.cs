using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Grade
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Value { get; set; }

    public decimal Fee { get; set; }

    public virtual ICollection<RequestTutorForm> RequestTutorForms { get; set; } = new List<RequestTutorForm>();

    public virtual ICollection<Tutor> Tutors { get; set; } = new List<Tutor>();
}
