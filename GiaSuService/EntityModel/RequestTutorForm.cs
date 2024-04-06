using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class RequestTutorForm
{
    public int Id { get; set; }

    public int Students { get; set; }

    public string? AdditionalDetail { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime ExpiredDate { get; set; }

    public string AddressDetail { get; set; } = null!;

    public int SubjectId { get; set; }

    public int GradeId { get; set; }

    public int StatusId { get; set; }

    public int DistrictId { get; set; }

    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual District District { get; set; } = null!;

    public virtual Grade Grade { get; set; } = null!;

    public virtual ICollection<RequestStatus> RequestStatuses { get; set; } = new List<RequestStatus>();

    public virtual Status Status { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();

    public virtual ICollection<TutorApplyForm> TutorApplyForms { get; set; } = new List<TutorApplyForm>();

    public virtual ICollection<SessionDate> Sessions { get; set; } = new List<SessionDate>();
}
