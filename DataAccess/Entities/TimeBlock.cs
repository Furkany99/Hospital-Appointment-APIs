using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class TimeBlock
{
    public int Id { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int RoutineId { get; set; }

    public virtual Routine Routine { get; set; } = null!;
}
