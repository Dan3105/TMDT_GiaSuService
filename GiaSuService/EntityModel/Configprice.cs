using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Configprice
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public double Rate { get; set; }

    public bool Type { get; set; }

    public int Gradeid { get; set; }

    public virtual Grade Grade { get; set; } = null!;
}
