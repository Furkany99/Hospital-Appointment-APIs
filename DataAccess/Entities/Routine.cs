using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Routine
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public int DayOfWeek { get; set; }

    public bool IsOnLeave { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<TimeBlock> TimeBlocks { get; set; } = new List<TimeBlock>();
}
