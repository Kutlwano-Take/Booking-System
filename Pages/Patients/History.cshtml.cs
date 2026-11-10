using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PracticeBookingSystem.Models;
using PracticeBookingSystem.Services;
using PracticeBookingSystem.Services.Interfaces;

namespace PracticeBookingSystem.Pages.Patients
{
    public class HistoryModel(PatientMemoryStore patients, IAppointmentService appointments, IPracticeDataService practiceData) : PageModel
    {
        public Patient? Patient { get; private set; }
        public IReadOnlyList<Appointment> History { get; private set; } = [];
        public IReadOnlyList<Practitioner> Practitioners => practiceData.GetPractitioners();
        public IReadOnlyList<PracticeLocation> Locations => practiceData.GetLocations();
        public IActionResult OnGet(int id)
        {
            Patient = patients.Get(id);
            if (Patient is null) 
                return NotFound();
            History = appointments.GetAll().Where(x => x.PatientId == id).
                OrderByDescending(x => x.StartsAt).
                ToList();
            return Page();
        }
    }
}
