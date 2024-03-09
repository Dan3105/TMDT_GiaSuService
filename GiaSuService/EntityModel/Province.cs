using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Province
{
    public int Id { get; set; }

    public string Provincename { get; set; } = null!;

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
