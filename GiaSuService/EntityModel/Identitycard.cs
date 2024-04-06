using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class IdentityCard
{
    public int Id { get; set; }

    public string IdentityNumber { get; set; } = null!;

    public string FrontIdentityCard { get; set; } = null!;

    public string BackIdentityCard { get; set; } = null!;

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Tutor? Tutor { get; set; }
}
