using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Account
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public string? FirebaseUid { get; set; }

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public virtual Role Role { get; set; } = null!;
}
