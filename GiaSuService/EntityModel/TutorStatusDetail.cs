using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class TutorStatusDetail
{
    public int Id { get; set; }

    public string Context { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public int TutorId { get; set; }

    public int StatusId { get; set; }

    public virtual Status Status { get; set; } = null!;

    public virtual Tutor Tutor { get; set; } = null!;
}
