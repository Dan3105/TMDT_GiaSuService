using GiaSuService.Configs;
using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Classprofile
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public short Nstudents { get; set; }

    public short Nsessions { get; set; }

    public string? Additionaldetail { get; set; }

    public float Commission { get; set; }

    public DateTime Createddate { get; set; }

    public DateTime? Refreshdate { get; set; }

    public int Addressid { get; set; }

    public int Sessionid { get; set; }

    public int Gradeid { get; set; }

    public int Subjectid { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual Grade Grade { get; set; } = null!;

    public virtual Sessiondate Session { get; set; } = null!;

    public virtual ICollection<Statushistoryclass> Statushistoryclasses { get; set; } = new List<Statushistoryclass>();

    public virtual Subject Subject { get; set; } = null!;

    public AppConfig.ClassStatus Classstatus { get; set; } = AppConfig.ClassStatus.PENDING;
}
