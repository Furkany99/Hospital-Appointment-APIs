using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Title
{
    public int Id { get; set; }

    public string? TitleName { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
