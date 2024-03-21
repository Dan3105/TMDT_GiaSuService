using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class District
{
    public int Id { get; set; }

    public string Districtname { get; set; } = null!;

    public int Provinceid { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Province Province { get; set; } = null!;

    public virtual ICollection<Tutorrequestform> Tutorrequestforms { get; set; } = new List<Tutorrequestform>();

    public virtual ICollection<Tutorprofile> Tutors { get; set; } = new List<Tutorprofile>();
}
