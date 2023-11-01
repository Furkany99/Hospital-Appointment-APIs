using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class OneTime
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public DateTime Day { get; set; }

    public bool IsOnLeave { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<OneTimeTimeBlock> OneTimeTimeBlocks { get; set; } = new List<OneTimeTimeBlock>();
}
