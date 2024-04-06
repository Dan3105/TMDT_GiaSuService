using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class TypeTransaction
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();
}
