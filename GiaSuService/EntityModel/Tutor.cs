using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Tutor
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public DateOnly? Birth { get; set; }

    public string Gender { get; set; } = null!;

    public string Addressdetail { get; set; } = null!;

    public string College { get; set; } = null!;

    public string Area { get; set; } = null!;

    public bool Typetutor { get; set; }

    public string? Additionalinfo { get; set; }

    public short Academicyearfrom { get; set; }

    public short Academicyearto { get; set; }

    public int Districtid { get; set; }

    public int Accountid { get; set; }

    public int Identityid { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual District District { get; set; } = null!;

    public virtual Identitycard Identity { get; set; } = null!;

    public virtual ICollection<Registerstatusdetail> Registerstatusdetails { get; set; } = new List<Registerstatusdetail>();

    public virtual ICollection<Tutorqueue> Tutorqueues { get; set; } = new List<Tutorqueue>();

    public virtual ICollection<District> Districts { get; set; } = new List<District>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Sessiondate> Sessions { get; set; } = new List<Sessiondate>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
