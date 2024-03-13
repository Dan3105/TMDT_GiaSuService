using GiaSuService.Configs;
using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Formregistertutor
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public DateOnly Birth { get; set; }

    public string Identitycard { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Passwordregister { get; set; } = null!;

    public int Tutorid { get; set; }

    public virtual Tutorprofile Tutor { get; set; } = null!;

    public virtual Tutorprofile? Tutorprofile { get; set; }

    public AppConfig.RegisterStatus registerStatus = AppConfig.RegisterStatus.PENDING;
}
