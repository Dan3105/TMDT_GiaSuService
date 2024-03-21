using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Grade
{
    public int Id { get; set; }

    public string Gradename { get; set; } = null!;

    public short Value { get; set; }

    public virtual ICollection<Configpricehistory> Configpricehistories { get; set; } = new List<Configpricehistory>();

    public virtual ICollection<Tutorrequestform> Tutorrequestforms { get; set; } = new List<Tutorrequestform>();

    public virtual ICollection<Tutorprofile> Tutors { get; set; } = new List<Tutorprofile>();
}
