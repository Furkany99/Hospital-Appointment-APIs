using System;
using System.Collections.Generic;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts;

public partial class HospitalAppointmentContext : DbContext
{
    public HospitalAppointmentContext()
    {
    }

    public HospitalAppointmentContext(DbContextOptions<HospitalAppointmentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentTime> AppointmentTimes { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<OneTime> OneTimes { get; set; }

    public virtual DbSet<OneTimeTimeBlock> OneTimeTimeBlocks { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Routine> Routines { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<TimeBlock> TimeBlocks { get; set; }

    public virtual DbSet<Title> Titles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-FI8FV2M;Database=Hospital_appointment;Trusted_Connection=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Login");

            entity.ToTable("Account");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Password)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Admin_1");

            entity.ToTable("Admin");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("Account_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Account).WithMany(p => p.Admins)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_Account");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("date");
            entity.Property(e => e.DepartmentId).HasColumnName("Department_ID");
            entity.Property(e => e.Detail).HasMaxLength(200);
            entity.Property(e => e.DocId).HasColumnName("Doc_ID");
            entity.Property(e => e.PatientId).HasColumnName("Patient_ID");
            entity.Property(e => e.StatusId).HasColumnName("Status_ID");

            entity.HasOne(d => d.Department).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Department");

            entity.HasOne(d => d.Doc).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Doctor");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Patient");

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Status");
        });

        modelBuilder.Entity<AppointmentTime>(entity =>
        {
            entity.ToTable("AppointmentTime");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentTimes)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppointmentTime_Appointment");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("Department_Name");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctor");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("Account_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TitleId).HasColumnName("Title_ID");

            entity.HasOne(d => d.Account).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctor_Account");

            entity.HasOne(d => d.Title).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.TitleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctor_Title");

            entity.HasMany(d => d.Departments).WithMany(p => p.Doctors)
                .UsingEntity<Dictionary<string, object>>(
                    "DoctorsDepartment",
                    r => r.HasOne<Department>().WithMany()
                        .HasForeignKey("DepartmentId")
                        .HasConstraintName("FK_DoctorsDepartmans_Department"),
                    l => l.HasOne<Doctor>().WithMany()
                        .HasForeignKey("DoctorId")
                        .HasConstraintName("FK_DoctorsDepartmans_Doctor"),
                    j =>
                    {
                        j.HasKey("DoctorId", "DepartmentId").HasName("PK_DoctorsDepartments_1");
                        j.ToTable("DoctorsDepartments");
                        j.IndexerProperty<int>("DoctorId").HasColumnName("Doctor_ID");
                        j.IndexerProperty<int>("DepartmentId").HasColumnName("Department_ID");
                    });
        });

        modelBuilder.Entity<OneTime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Leave");

            entity.ToTable("OneTime");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Day).HasColumnType("date");
            entity.Property(e => e.DoctorId).HasColumnName("Doctor_ID");
            entity.Property(e => e.IsOnLeave).HasColumnName("isOnLeave");

            entity.HasOne(d => d.Doctor).WithMany(p => p.OneTimes)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OneTime_Doctor");
        });

        modelBuilder.Entity<OneTimeTimeBlock>(entity =>
        {
            entity.ToTable("OneTimeTimeBlock");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OneTimeId).HasColumnName("OneTime_ID");

            entity.HasOne(d => d.OneTime).WithMany(p => p.OneTimeTimeBlocks)
                .HasForeignKey(d => d.OneTimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OneTimeTimeBlock_OneTime");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patient");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("Account_ID");
            entity.Property(e => e.Birthdate).HasColumnType("date");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Account).WithMany(p => p.Patients)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patient_Account");
        });

        modelBuilder.Entity<Routine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Schedules");

            entity.ToTable("Routine");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DoctorId).HasColumnName("Doctor_ID");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Routines)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Routine_Doctor");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TimeBlock>(entity =>
        {
            entity.ToTable("TimeBlock");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RoutineId).HasColumnName("RoutineID");

            entity.HasOne(d => d.Routine).WithMany(p => p.TimeBlocks)
                .HasForeignKey(d => d.RoutineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TimeBlock_Routine");
        });

        modelBuilder.Entity<Title>(entity =>
        {
            entity.ToTable("Title");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.TitleName)
                .HasMaxLength(50)
                .HasColumnName("Title_Name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
