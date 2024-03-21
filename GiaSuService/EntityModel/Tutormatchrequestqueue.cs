using GiaSuService.Configs;
using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Tutormatchrequestqueue
{
    public int Formid { get; set; }

    public int Tutorid { get; set; }

    public AppConfig.QueueStatus Queuestatus { get; set; } = AppConfig.QueueStatus.PENDING;

    public DateOnly? Applydate { get; set; }

    public virtual Tutorrequestform Form { get; set; } = null!;

    public virtual Tutorprofile Tutor { get; set; } = null!;
}
