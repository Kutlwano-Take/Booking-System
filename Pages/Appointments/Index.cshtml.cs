using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PracticeBookingSystem.Models;
using PracticeBookingSystem.Services.Interfaces;

namespace PracticeBookingSystem.Pages.Appointments
{
    public class IndexModel(IPracticeDataService practiceData, IAppointmentService appointments, INotificationService notifications) : PageModel
    {
        public DateTime SelectedDate { get; private set; } = DateTime.Today;
        public IReadOnlyList<Practitioner> Practitioners => practiceData.GetPractitioners();
        public IReadOnlyList<Patient> Patients => practiceData.GetPatients();
        public IReadOnlyList<PracticeLocation> Locations => practiceData.GetLocations();
        public IReadOnlyList<Appointment> DayAppointments { get; private set; } = [];
        [BindProperty] public CreateAppointmentRequest Booking { get; set; } = new();
        [BindProperty] public int CancelId { get; set; }
        [BindProperty] public string CancelReason { get; set; } = "";
        public void OnGet(DateTime? date) 
        {
            SelectedDate = (date ?? DateTime.Today).Date; 
            DayAppointments = appointments.GetAll()
                .Where(a => a.StartsAt.Date == SelectedDate)
                .ToList(); 
        }
        public async Task<IActionResult> OnPostBookAsync(CancellationToken ct)
        {
            if (!ModelState.IsValid) { 
                OnGet(Booking.StartsAt.Date); 
                return Page(); 
            }
            var result = appointments.Book(Booking);

            if (!result.Succeeded) 
            { 
                ModelState.AddModelError(string.Empty, result.Error ?? "Booking failed."); 
                OnGet(Booking.StartsAt.Date); 
                return Page(); 
            }

            await notifications.SendConfirmationAsync(result.Appointment!, ct);

            result.Appointment!.ConfirmationEmailSent = true;
            TempData["Message"] = $"Booking #{result.Appointment.Id} confirmed. Demo email recorded.";
            return RedirectToPage("/Appointments/Index", new { date = result.Appointment.StartsAt.ToString("yyyy-MM-dd") });
        }
        public async Task<IActionResult> OnPostCancelAsync(CancellationToken ct)
        {
            var result = appointments.Cancel(CancelId, CancelReason);
            if (!result.Succeeded) 
            { 
                ModelState.AddModelError(string.Empty, result.Error ?? "Cancellation failed.");
                OnGet(result.Appointment?.StartsAt.Date ?? DateTime.Today); 
                return Page(); 
            }
            await notifications.SendCancellationAsync(result.Appointment!, CancelReason, ct);
            TempData["Message"] = $"Appointment #{CancelId} cancelled.";
            return RedirectToPage("/Appointments/Index", new { date = result.Appointment!.StartsAt.ToString("yyyy-MM-dd") });
        }
    }
}