using GiaSuService.Configs;
using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Classtutorqueue
{
    public int Classid { get; set; }

    public int Tutorid { get; set; }

    public decimal Price { get; set; }

    public virtual Classprofile Class { get; set; } = null!;

    public virtual Tutorprofile Tutor { get; set; } = null!;

    public AppConfig.ClassTutorQueue ClassTutorQueue { get; set; } = AppConfig.ClassTutorQueue.PENDING;
    public AppConfig.PaymentMethod PaymentMethod { get; set; } = AppConfig.PaymentMethod.OFFLINE;
}
