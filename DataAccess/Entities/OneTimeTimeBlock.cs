using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class OneTimeTimeBlock
{
    public int Id { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int OneTimeId { get; set; }

    public virtual OneTime OneTime { get; set; } = null!;
}
