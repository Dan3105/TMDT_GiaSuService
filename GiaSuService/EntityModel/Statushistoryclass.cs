using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Statushistoryclass
{
    public int Statusid { get; set; }

    public int Classid { get; set; }

    public int Commitername { get; set; }

    public string Statusdetail { get; set; } = null!;

    public virtual Classprofile Class { get; set; } = null!;
}
