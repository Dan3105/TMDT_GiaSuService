using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Employee
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly Birth { get; set; }

    public string Gender { get; set; } = null!;

    public string AddressDetail { get; set; } = null!;

    public int DistrictId { get; set; }

    public int AccountId { get; set; }

    public int IdentityId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual District District { get; set; } = null!;

    public virtual IdentityCard Identity { get; set; } = null!;

    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();
}
