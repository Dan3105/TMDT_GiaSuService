using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Address
{
    public int Id { get; set; }

    public string Addressname { get; set; } = null!;

    public int Districtid { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Classprofile? Classprofile { get; set; }

    public virtual District District { get; set; } = null!;
}
