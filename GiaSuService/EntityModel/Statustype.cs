using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class StatusType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Status> Statuses { get; set; } = new List<Status>();
}
