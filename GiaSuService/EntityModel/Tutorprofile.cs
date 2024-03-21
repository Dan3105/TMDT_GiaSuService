using GiaSuService.Configs;
using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Tutorprofile
{
    public int Id { get; set; }

    public string College { get; set; } = null!;

    public string Area { get; set; } = null!;

    public string? Additionalinfo { get; set; }

    public int Accountid { get; set; }

    public short Academicyearfrom { get; set; }

    public short Academicyearto { get; set; }

    public virtual Account Account { get; set; } = null!;

    public AppConfig.TypeTutor Currentstatus;

    public AppConfig.RegisterStatus Formstatus { get; set; } = AppConfig.RegisterStatus.PENDING;

    public virtual ICollection<Tutormatchrequestqueue> Tutormatchrequestqueues { get; set; } = new List<Tutormatchrequestqueue>();

    public virtual ICollection<Tutorrequestform> Tutorrequestforms { get; set; } = new List<Tutorrequestform>();

    public virtual ICollection<District> Districts { get; set; } = new List<District>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Sessiondate> Sessions { get; set; } = new List<Sessiondate>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
