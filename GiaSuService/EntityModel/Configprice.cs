using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class ConfigPrice
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public int GradeId { get; set; }

    public virtual Grade Grade { get; set; } = null!;
}
