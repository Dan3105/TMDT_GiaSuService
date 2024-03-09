using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Role
{
    public int Id { get; set; }

    public string Rolename { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
