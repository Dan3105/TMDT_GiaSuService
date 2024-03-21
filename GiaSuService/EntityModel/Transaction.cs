using GiaSuService.Configs;
using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Transaction
{
    public int Id { get; set; }

    public string? Context { get; set; }

    public decimal Price { get; set; }

    public double? Commission { get; set; }

    public AppConfig.TransactionType Transactiontype;

    public AppConfig.PaymentMethod Paymentmethod { get; set; }

    public DateTime? Createdate { get; set; }

    public DateTime? Paymentdeadline { get; set; }

    public DateTime? Paymentdate { get; set; }

    public int? Accountpayid { get; set; }

    public int? Accountwriteid { get; set; }

    public int? Tutorformid { get; set; }

    public virtual Account? Accountpay { get; set; }

    public virtual Account? Accountwrite { get; set; }

    public virtual Tutorrequestform? Tutorform { get; set; }
}
