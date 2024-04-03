using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Registerstatusdetail
{
    public int Id { get; set; }

    public int Tutorid { get; set; }

    public string Context { get; set; } = null!;

    public int Statusid { get; set; }

    public DateTime Reviewdate { get; set; }

    public virtual Status Status { get; set; } = null!;

    public virtual Tutor Tutor { get; set; } = null!;
}
