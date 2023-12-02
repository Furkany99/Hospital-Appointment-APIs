using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class AppointmentTime
{
    public int Id { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int AppointmentId { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
}
