using Microsoft.EntityFrameworkCore;
using PracticeBookingSystem.Models;

namespace PracticeBookingSystem.Data;

public sealed class PracticeBookingDbContext(DbContextOptions<PracticeBookingDbContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Practitioner> Practitioners => Set<Practitioner>();
    public DbSet<PracticeLocation> Locations => Set<PracticeLocation>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(24);
            entity.Property(x => x.Notes).HasMaxLength(1000);
            entity.HasIndex(x => new { x.PractitionerId, x.StartsAt, x.EndsAt, x.Status });
        });
        modelBuilder.Entity<Patient>().ToTable("Patients").HasKey(x => x.Id);
        modelBuilder.Entity<Practitioner>().ToTable("Practitioners").HasKey(x => x.Id);
        modelBuilder.Entity<PracticeLocation>().ToTable("PracticeLocations").HasKey(x => x.Id);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.JobTitle).HasMaxLength(150);
            entity.Property(x => x.PasswordHash).HasMaxLength(512);
            entity.Property(x => x.GoogleSubject).HasMaxLength(100);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.GoogleSubject).IsUnique(); // nullable, so many NULLs are allowed on SQL Server
        });
    }
}
