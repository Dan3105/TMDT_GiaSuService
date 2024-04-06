using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class RequestStatus
{
    public int Id { get; set; }

    public string Context { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public int TutorRequestId { get; set; }

    public virtual RequestTutorForm TutorRequest { get; set; } = null!;
}
