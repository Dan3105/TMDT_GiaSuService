using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Tutorrequestform
{
    public int Id { get; set; }

    public int Students { get; set; }

    public string? Additionaldetail { get; set; }

    public DateTime Createddate { get; set; }

    public DateTime Expireddate { get; set; }

    public int Statusid { get; set; }

    public string Addressdetail { get; set; } = null!;

    public int Districtid { get; set; }

    public int Subjectid { get; set; }

    public int Gradeid { get; set; }

    public int Customerid { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual District District { get; set; } = null!;

    public virtual Grade Grade { get; set; } = null!;

    public virtual ICollection<Requeststatus> Requeststatuses { get; set; } = new List<Requeststatus>();

    public virtual Status Status { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<Transactionhistory> Transactionhistoryhistories { get; set; } = new List<Transactionhistory>();

    public virtual ICollection<Tutorqueue> Tutorqueues { get; set; } = new List<Tutorqueue>();

    public virtual ICollection<Sessiondate> Sessions { get; set; } = new List<Sessiondate>();
}
