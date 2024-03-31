using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Status
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Statustypeid { get; set; }

    public virtual ICollection<Registerstatusdetail> Registerstatusdetails { get; set; } = new List<Registerstatusdetail>();

    public virtual Statustype Statustype { get; set; } = null!;

    public virtual ICollection<Tutorqueue> Tutorqueues { get; set; } = new List<Tutorqueue>();

    public virtual ICollection<Tutorrequestform> Tutorrequestforms { get; set; } = new List<Tutorrequestform>();
}
