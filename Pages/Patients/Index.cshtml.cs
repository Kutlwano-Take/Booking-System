using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PracticeBookingSystem.Models;
using PracticeBookingSystem.Services;

namespace PracticeBookingSystem.Pages.Patients
{
    public class IndexModel(PatientMemoryStore store) : PageModel
    {
        public IReadOnlyList<Patient> Patients { get; private set; } = [];
        
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }
        [BindProperty] public Patient NewPatient { get; set; } = new();
        public void OnGet() => Patients = store.GetAll(Search);
        public IActionResult OnPostCreate()
        {
            if (!ModelState.IsValid) { 
                Patients = store.GetAll(Search);
                return Page(); 
            }

            store.Add(NewPatient); 

            return RedirectToPage();
        }
    }
}