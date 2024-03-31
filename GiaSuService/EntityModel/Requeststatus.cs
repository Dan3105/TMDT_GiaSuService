using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Requeststatus
{
    public int Id { get; set; }

    public string Context { get; set; } = null!;

    public DateOnly? Createdate { get; set; }

    public int Tutorrequestid { get; set; }

    public virtual Tutorrequestform Tutorrequest { get; set; } = null!;
}
