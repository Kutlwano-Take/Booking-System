using PracticeBookingSystem.Models;
namespace PracticeBookingSystem.Services.Interfaces;
public interface IPracticeDataService
{
    IReadOnlyList<Patient> GetPatients();
    IReadOnlyList<Practitioner> GetPractitioners();
    IReadOnlyList<PracticeLocation> GetLocations();
}
