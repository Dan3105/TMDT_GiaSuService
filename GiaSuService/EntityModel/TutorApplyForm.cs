using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class TutorApplyForm
{
    public int TutorRequestId { get; set; }

    public int TutorId { get; set; }

    public DateTime? EnterDate { get; set; }

    public int StatusId { get; set; }

    public virtual Status Status { get; set; } = null!;

    public virtual Tutor Tutor { get; set; } = null!;

    public virtual RequestTutorForm TutorRequest { get; set; } = null!;
}
