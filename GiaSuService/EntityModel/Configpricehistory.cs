using GiaSuService.Configs;
using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Configpricehistory
{
    public int Id { get; set; }

    public int Nsessions { get; set; }

    public int? Gradeid { get; set; }

    public decimal? Price { get; set; }

    public double? Commission { get; set; }

    public AppConfig.TypeTutor Typetutor { get; set; }

    public DateOnly? Createdate { get; set; }

    public int? Accountwriteid { get; set; }

    public virtual Account? Accountwrite { get; set; }

    public virtual Grade? Grade { get; set; }
}
