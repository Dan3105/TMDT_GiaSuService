using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Account
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public bool? Lockenable { get; set; }

    public string Avatar { get; set; } = null!;

    public DateTime? Createdate { get; set; }

    public int Roleid { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Transactionhistory> Transactionhistoryhistories { get; set; } = new List<Transactionhistory>();

    public virtual Tutor? Tutor { get; set; }
}
