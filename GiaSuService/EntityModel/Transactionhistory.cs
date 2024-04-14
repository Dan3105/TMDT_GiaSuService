using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class TransactionHistory
{
    public int Id { get; set; }

    public bool TypeTransaction { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal PaymentAmount { get; set; }

    public string? Context { get; set; }

    public int EmployeeId { get; set; }

    public int TutorId { get; set; }

    public int FormId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual RequestTutorForm Form { get; set; } = null!;

    public virtual Tutor Tutor { get; set; } = null!;
}
