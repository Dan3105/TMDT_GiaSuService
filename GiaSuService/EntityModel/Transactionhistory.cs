using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Transactionhistory
{
    public int Id { get; set; }

    public int Typetransactionid { get; set; }

    public DateOnly? Createdate { get; set; }

    public DateOnly? Paymentdate { get; set; }

    public decimal Actualamount { get; set; }

    public decimal Payamount { get; set; }

    public double? Rate { get; set; }

    public string? Context { get; set; }

    public int Employeeid { get; set; }

    public int Accountid { get; set; }

    public int Formid { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Tutorrequestform Form { get; set; } = null!;

    public virtual Typetransaction Typetransaction { get; set; } = null!;
}
