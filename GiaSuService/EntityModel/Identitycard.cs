using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Identitycard
{
    public int Id { get; set; }

    public string Identitynumber { get; set; } = null!;

    public string Frontidentitycard { get; set; } = null!;

    public string Backidentitycard { get; set; } = null!;

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Tutor? Tutor { get; set; }
}
