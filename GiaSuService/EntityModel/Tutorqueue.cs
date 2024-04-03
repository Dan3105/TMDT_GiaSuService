using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Tutorqueue
{
    public int Tutorrequestid { get; set; }

    public int Tutorid { get; set; }

    public DateTime? Enterdate { get; set; }

    public int Statusid { get; set; }

    public virtual Status Status { get; set; } = null!;

    public virtual Tutor Tutor { get; set; } = null!;

    public virtual Tutorrequestform Tutorrequest { get; set; } = null!;
}
