using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Tutor
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly Birth { get; set; }

    public string Gender { get; set; } = null!;

    public string AddressDetail { get; set; } = null!;

    public string College { get; set; } = null!;

    public string Area { get; set; } = null!;

    public string? AdditionalInfo { get; set; }

    public short AcademicYearFrom { get; set; }

    public short AcademicYearTo { get; set; }

    public bool? IsActive { get; set; } = true;

    public int DistrictId { get; set; }

    public int AccountId { get; set; }

    public int IdentityId { get; set; }

    public int TutorTypeId { get; set; }

    public int StatusId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual District District { get; set; } = null!;

    public virtual IdentityCard Identity { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual ICollection<TutorApplyForm> TutorApplyForms { get; set; } = new List<TutorApplyForm>();

    public virtual ICollection<TutorStatusDetail> TutorStatusDetails { get; set; } = new List<TutorStatusDetail>();

    public virtual TutorType TutorType { get; set; } = null!;

    public virtual ICollection<District> Districts { get; set; } = new List<District>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<SessionDate> Sessions { get; set; } = new List<SessionDate>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
