using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Patient
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTime Birthdate { get; set; }

    public string? PhoneNumber { get; set; }

    public double? Height { get; set; }

    public int? Weight { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
