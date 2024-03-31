using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Typetransaction
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Transactionhistory> Transactionhistoryhistories { get; set; } = new List<Transactionhistory>();
}
