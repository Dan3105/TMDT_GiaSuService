using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class District
{
    public int Id { get; set; }

    public string Districtname { get; set; } = null!;

    public int Provinceid { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Classprofile> Classprofiles { get; set; } = new List<Classprofile>();

    public virtual Province Province { get; set; } = null!;

    public virtual ICollection<Tutorprofile> Tutors { get; set; } = new List<Tutorprofile>();
}
