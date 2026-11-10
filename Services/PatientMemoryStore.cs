using Microsoft.EntityFrameworkCore;
using PracticeBookingSystem.Data;
using PracticeBookingSystem.Models;

namespace PracticeBookingSystem.Services;

public sealed class PatientMemoryStore
{
    private readonly PracticeBookingDbContext _db;

    public PatientMemoryStore(PracticeBookingDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<Patient> GetAll(string? search = null)
    {
        var query = _db.Patients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.FullName.Contains(search) ||
                (x.Email != null && x.Email.Contains(search)) ||
                (x.FileNumber != null && x.FileNumber.Contains(search)) ||
                x.Id.ToString() == search);
        }

        return query
            .OrderBy(x => x.FullName)
            .ToList();
    }

    public Patient? Get(int id)
    {
        return _db.Patients
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public Patient Add(Patient patient)
    {
        // Let SQL Server generate the primary key.
        patient.Id = 0;

        _db.Patients.Add(patient);
        _db.SaveChanges();

        // Generate a patient file number after SQL Server generates the ID.
        if (string.IsNullOrWhiteSpace(patient.FileNumber))
        {
            patient.FileNumber = $"PT-{patient.Id}";

            _db.SaveChanges();
        }

        return patient;
    }

    public void Update(Patient patient)
    {
        var current = _db.Patients
            .FirstOrDefault(x => x.Id == patient.Id);

        if (current is null)
            return;

        current.FullName = patient.FullName;
        current.Email = patient.Email;
        current.Phone = patient.Phone;
        current.IdentityNumber = patient.IdentityNumber;
        current.FileNumber = patient.FileNumber;
        current.MedicalAid = patient.MedicalAid;
        current.DateOfBirth = patient.DateOfBirth;
        current.Notes = patient.Notes;

        _db.SaveChanges();
    }
}
