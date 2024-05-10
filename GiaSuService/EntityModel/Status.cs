using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Status
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string VietnameseName { get; set; } = null!;

    public int StatusTypeId { get; set; }

    public virtual ICollection<RequestTutorForm> RequestTutorForms { get; set; } = new List<RequestTutorForm>();

    public virtual StatusType StatusType { get; set; } = null!;

    public virtual ICollection<TutorApplyForm> TutorApplyForms { get; set; } = new List<TutorApplyForm>();

    public virtual ICollection<TutorStatusDetail> TutorStatusDetails { get; set; } = new List<TutorStatusDetail>();

    public virtual ICollection<Tutor> Tutors { get; set; } = new List<Tutor>();

    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();
}
