using System;
using System.Collections.Generic;

namespace AppOrderNilon.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public string ReportType { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime GeneratedDate { get; set; }

    public string? Data { get; set; }

    public int? AdminId { get; set; }

    public virtual Admin? Admin { get; set; }
}
