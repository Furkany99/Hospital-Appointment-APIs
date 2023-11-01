using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataAccess.Entities;

public partial class Doctor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int TitleId { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<OneTime> OneTimes { get; set; } = new List<OneTime>();

	public virtual ICollection<Routine> Routines { get; set; } = new List<Routine>();

    public virtual Title Title { get; set; } = null!;

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
