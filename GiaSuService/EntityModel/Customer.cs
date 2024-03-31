using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Customer
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public DateOnly? Birth { get; set; }

    public string Gender { get; set; } = null!;

    public string Addressdetail { get; set; } = null!;

    public int Districtid { get; set; }

    public int Accountid { get; set; }

    public int Identityid { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual District District { get; set; } = null!;

    public virtual Identitycard Identity { get; set; } = null!;

    public virtual ICollection<Tutorrequestform> Tutorrequestforms { get; set; } = new List<Tutorrequestform>();
}
