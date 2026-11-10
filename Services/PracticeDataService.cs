using Microsoft.EntityFrameworkCore;
using PracticeBookingSystem.Data;
using PracticeBookingSystem.Models;
using PracticeBookingSystem.Services.Interfaces;
namespace PracticeBookingSystem.Services;
public sealed class PracticeDataService(PracticeBookingDbContext db) : IPracticeDataService
{
    public IReadOnlyList<Patient> GetPatients() => db.Patients.AsNoTracking().OrderBy(x => x.FullName).ToList();
    public IReadOnlyList<Practitioner> GetPractitioners() => db.Practitioners.AsNoTracking().OrderBy(x => x.FullName).ToList();
    public IReadOnlyList<PracticeLocation> GetLocations() => db.Locations.AsNoTracking().OrderBy(x => x.Name).ToList();
}
