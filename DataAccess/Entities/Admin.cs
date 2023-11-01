using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Admin
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
