using GiaSuService.Configs;
using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class Formregistertutor
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public DateOnly Birth { get; set; }

    public string Identitycard { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string College { get; set; } = null!;

    public string Area { get; set; } = null!;

    public string Currentstatus { get; set; } = null!;

    public short Yeargraduate { get; set; }

    public string? Additionaldetail { get; set; }

    public string Passwordregister { get; set; } = null!;

    public AppConfig.RegisterStatus registerStatus = AppConfig.RegisterStatus.PENDING;
}
