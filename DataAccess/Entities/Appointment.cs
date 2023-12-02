using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Appointment
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int DepartmentId { get; set; }

    public string? Detail { get; set; }

    public int DocId { get; set; }

    public int PatientId { get; set; }

    public int StatusId { get; set; }

    public virtual ICollection<AppointmentTime> AppointmentTimes { get; set; } = new List<AppointmentTime>();

    public virtual Department Department { get; set; } = null!;

    public virtual Doctor Doc { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
