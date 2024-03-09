using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Address
{
    public int Id { get; set; }

    public string Addressname { get; set; } = null!;

    public int Districtid { get; set; }

    public int Accountid { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Classprofile> Classprofiles { get; set; } = new List<Classprofile>();

    public virtual District District { get; set; } = null!;
}
