using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class District
{
    public int Id { get; set; }

    public string Districtname { get; set; } = null!;

    public int Provinceid { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Province Province { get; set; } = null!;

    public virtual ICollection<Tutorprofile> Tutors { get; set; } = new List<Tutorprofile>();
}
