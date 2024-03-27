using GiaSuService.Configs;
using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Tutorrequestform
{
    public int Id { get; set; }

    public short Nstudents { get; set; }

    public short Nsessions { get; set; }

    public string? Additionaldetail { get; set; }

    public DateTime Createddate { get; set; }

    public DateTime Expireddate { get; set; }

    public int Gradeid { get; set; }

    public int Subjectid { get; set; }

    public string Addressdetail { get; set; } = null!;

    public int Districtid { get; set; }

    public int Accountid { get; set; }

    public int? Tutorid { get; set; }

    public virtual District District { get; set; } = null!;

    public virtual Grade Grade { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual Tutorprofile? Tutor { get; set; }

    public AppConfig.TutorRequestStatus Status = AppConfig.TutorRequestStatus.PENDING;

    public virtual ICollection<Tutormatchrequestqueue> Tutormatchrequestqueues { get; set; } = new List<Tutormatchrequestqueue>();

    public virtual ICollection<Tutorrequesthistorystatus> Tutorrequesthistorystatuses { get; set; } = new List<Tutorrequesthistorystatus>();
}
