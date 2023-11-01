using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class DoctorsDepartment
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public int DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;
}
