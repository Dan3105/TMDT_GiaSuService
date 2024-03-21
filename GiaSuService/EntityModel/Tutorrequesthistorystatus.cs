using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Tutorrequesthistorystatus
{
    public int Id { get; set; }

    public int Tutorrequestformid { get; set; }

    public int Commitername { get; set; }

    public string Statusdetail { get; set; } = null!;

    public virtual Tutorrequestform Tutorrequestform { get; set; } = null!;
}
