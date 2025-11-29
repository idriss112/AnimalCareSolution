using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AnimalCare.Data;
using AnimalCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimalCare.Controllers
{
    // All appointment actions require user to be logged in
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly AnimalCareDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(AnimalCareDbContext context, ILogger<AppointmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Index view with filtering:
        /// - Admin + Receptionist: see all appointments (with filters)
        /// - Veterinarian: see only their appointments (with filters)
        /// </summary>
        public async Task<IActionResult> Index(DateTime? date, string search)
        {
            // Get current user ID (from Identity)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Load user including linked Veterinarian (if any)
            var user = await _context.Users
                .Include(u => u.Veterinarian)
                .FirstOrDefaultAsync(u => u.Id == userId);

            // Base query with includes so we have all related data
            IQueryable<Appointment> query = _context.Appointments
                .Include(a => a.Animal)
                    .ThenInclude(an => an.Owner)
                .Include(a => a.Veterinarian);

            // If current user is a Veterinarian, restrict to their appointments only
            if (User.IsInRole("Veterinarian") && user?.VeterinarianId != null)
            {
                int vetId = user.VeterinarianId.Value;
                query = query.Where(a => a.VeterinarianId == vetId);
            }

            // Apply date filter if provided
            if (date.HasValue)
            {
                var filterDate = date.Value.Date;
                query = query.Where(a => a.AppointmentDateTime.Date == filterDate);
            }

            // Apply search filter if provided (search in animal name or owner name)
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(a =>
                    a.Animal.Name.ToLower().Contains(search) ||
                    (a.Animal.Owner != null && (
                        a.Animal.Owner.FirstName.ToLower().Contains(search) ||
                        a.Animal.Owner.LastName.ToLower().Contains(search) ||
                        (a.Animal.Owner.FirstName + " " + a.Animal.Owner.LastName).ToLower().Contains(search)
                    ))
                );
            }

            // Order by date and time
            query = query.OrderBy(a => a.AppointmentDateTime);

            var appointments = await query.ToListAsync();

            // Pass filter values to view
            ViewBag.FilterDate = date;
            ViewBag.FilterSearch = search;

            return View(appointments);
        }

        /// <summary>
        /// Details – everyone logged-in can view, but:
        /// - Vets should only see their own appointments (if you want, you can add extra checks)
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Animal)
                    .ThenInclude(an => an.Owner)
                .Include(a => a.Veterinarian)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        /// <summary>
        /// GET Create – Admin + Receptionist only
        /// </summary>
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View();
        }

        /// <summary>
        /// POST Create – with full business rules:
        /// - No past bookings
        /// - Vet must be available (schedule)
        /// - No double-booking
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create(
            [Bind("AnimalId,VeterinarianId,AppointmentDateTime,DurationMinutes,Reason,Status")]
    Appointment appointment)
        {
            // Remove navigation properties from validation
            ModelState.Remove("Animal");
            ModelState.Remove("Veterinarian");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedAt");

            // Basic validation: ensure IDs > 0
            if (appointment.AnimalId <= 0)
            {
                ModelState.AddModelError("AnimalId", "Please select an animal.");
            }
            if (appointment.VeterinarianId <= 0)
            {
                ModelState.AddModelError("VeterinarianId", "Please select a veterinarian.");
            }
            if (appointment.DurationMinutes <= 0)
            {
                ModelState.AddModelError("DurationMinutes", "Duration must be greater than 0.");
            }
            if (ModelState.IsValid)
            {
                // BUSINESS RULE 1: Cannot book in the past
                if (appointment.AppointmentDateTime < DateTime.Now)
                {
                    ModelState.AddModelError("AppointmentDateTime", "Cannot book an appointment in the past.");
                }
                else
                {
                    // BUSINESS RULE 2: Vet must be available (has a schedule covering this time)
                    bool vetAvailable = await IsVetAvailable(
                        appointment.VeterinarianId,
                        appointment.AppointmentDateTime,
                        appointment.DurationMinutes);
                    if (!vetAvailable)
                    {
                        ModelState.AddModelError("AppointmentDateTime",
                            "The veterinarian is not available at the selected time.");
                    }
                    else
                    {
                        // BUSINESS RULE 3: No double-booking – check for conflicting appointments
                        bool hasConflict = await HasAppointmentConflict(
                            appointment.VeterinarianId,
                            appointment.AppointmentDateTime,
                            appointment.DurationMinutes,
                            null); // new appointment, no ID yet
                        if (hasConflict)
                        {
                            ModelState.AddModelError("AppointmentDateTime",
                                "This veterinarian already has an appointment that overlaps with this time.");
                        }
                    }
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        appointment.CreatedAt = DateTime.Now;
                        appointment.UpdatedAt = DateTime.Now;
                        _context.Add(appointment);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Appointment created successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, "Error creating appointment");
                        TempData["ErrorMessage"] = "An error occurred while saving the appointment.";
                    }
                }
            }
            // If we reach here → there was a validation error
            await PopulateDropDowns(appointment.AnimalId, appointment.VeterinarianId);
            return View(appointment);
        }

        /// <summary>
        /// GET Edit – Admin + Receptionist only
        /// </summary>
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            await PopulateDropDowns(appointment.AnimalId, appointment.VeterinarianId);
            return View(appointment);
        }

        /// <summary>
        /// POST Edit – similar rules as Create
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,AnimalId,VeterinarianId,AppointmentDateTime,DurationMinutes,Reason,Status")]
            Appointment appointment)
        {
            if (id != appointment.Id)
                return NotFound();

            ModelState.Remove("Animal");
            ModelState.Remove("Veterinarian");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedAt");

            if (appointment.AnimalId <= 0)
            {
                ModelState.AddModelError("AnimalId", "Please select an animal.");
            }

            if (appointment.VeterinarianId <= 0)
            {
                ModelState.AddModelError("VeterinarianId", "Please select a veterinarian.");
            }

            if (appointment.DurationMinutes <= 0)
            {
                ModelState.AddModelError("DurationMinutes", "Duration must be greater than 0.");
            }

            if (ModelState.IsValid)
            {
                // No past bookings
                if (appointment.AppointmentDateTime < DateTime.Now)
                {
                    ModelState.AddModelError("AppointmentDateTime", "Cannot set an appointment in the past.");
                }
                else
                {
                    bool vetAvailable = await IsVetAvailable(
                        appointment.VeterinarianId,
                        appointment.AppointmentDateTime,
                        appointment.DurationMinutes);

                    if (!vetAvailable)
                    {
                        ModelState.AddModelError("AppointmentDateTime",
                            "The veterinarian is not available at the selected time.");
                    }
                    else
                    {
                        // Exclude this appointment from conflict check (we are editing it)
                        bool hasConflict = await HasAppointmentConflict(
                            appointment.VeterinarianId,
                            appointment.AppointmentDateTime,
                            appointment.DurationMinutes,
                            appointment.Id);

                        if (hasConflict)
                        {
                            ModelState.AddModelError("AppointmentDateTime",
                                "This veterinarian already has another appointment overlapping this time.");
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        appointment.UpdatedAt = DateTime.Now;

                        _context.Update(appointment);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Appointment updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!AppointmentExists(appointment.Id))
                        {
                            return NotFound();
                        }

                        _logger.LogError(ex, "Concurrency error editing appointment {AppointmentId}", appointment.Id);
                        TempData["ErrorMessage"] = "A concurrency error occurred while updating the appointment.";
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, "DB error editing appointment {AppointmentId}", appointment.Id);
                        TempData["ErrorMessage"] = "An error occurred while saving the appointment.";
                    }
                }
            }

            await PopulateDropDowns(appointment.AnimalId, appointment.VeterinarianId);
            return View(appointment);
        }

        /// <summary>
        /// GET Delete – confirm deletion
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Animal)
                    .ThenInclude(an => an.Owner)
                .Include(a => a.Veterinarian)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }


        /// <summary>
        /// For veterinarians: see only their own appointments, grouped by date.
        /// </summary>
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .Include(u => u.Veterinarian)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.VeterinarianId == null)
            {
                TempData["ErrorMessage"] = "No veterinarian profile linked to this user.";
                return RedirectToAction(nameof(Index));
            }

            int vetId = user.VeterinarianId.Value;

            var appointments = await _context.Appointments
                .Include(a => a.Animal)
                    .ThenInclude(an => an.Owner)
                .Include(a => a.Veterinarian)
                .Where(a => a.VeterinarianId == vetId)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();

            return View(appointments);
        }

        /// <summary>
        /// For Admin / Receptionist: today's full clinic schedule.
        /// </summary>
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> TodaysSchedule()
        {
            var today = DateTime.Today;

            var appointments = await _context.Appointments
                .Include(a => a.Animal)
                    .ThenInclude(an => an.Owner)
                .Include(a => a.Veterinarian)
                .Where(a => a.AppointmentDateTime.Date == today)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();

            return View(appointments);
        }



        /// <summary>
        /// POST Delete – here we physically delete appointment.
        /// In a real clinic, better is to just mark as Cancelled.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            try
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Appointment deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting appointment {AppointmentId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the appointment.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Helper – checks if vet has a schedule that covers the requested time.
        /// </summary>
        /// <summary>
        /// Helper – checks if vet has a schedule that covers the requested time.
        /// Updated for new recurring weekly schedule system (DayOfWeek only, 8 AM - 4 PM)
        /// </summary>
        private async Task<bool> IsVetAvailable(int veterinarianId, DateTime appointmentDateTime, int durationMinutes)
        {
            var appointmentDayOfWeek = appointmentDateTime.DayOfWeek;
            var appointmentStart = appointmentDateTime.TimeOfDay;
            var appointmentEnd = appointmentStart.Add(TimeSpan.FromMinutes(durationMinutes));

            // Get all active schedules for this vet on this day of week
            var schedules = await _context.VetSchedules
                .Where(s => s.VeterinarianId == veterinarianId &&
                            s.IsActive &&
                            s.DayOfWeek == appointmentDayOfWeek)
                .ToListAsync();

            if (!schedules.Any())
            {
                // No schedule for this day of week ⇒ vet not available
                return false;
            }

            // Appointment must fit inside at least one schedule interval
            // (In new system, all schedules are 8:00 AM - 4:00 PM, but we check anyway)
            bool fitsAnySchedule = schedules.Any(s =>
                appointmentStart >= s.StartTime &&
                appointmentEnd <= s.EndTime
            );

            return fitsAnySchedule;
        }


        /// <summary>
        /// Helper – checks overlapping appointments for a vet.
        /// Overlap rule:
        /// (Start1 < End2) AND (Start2 < End1)  ⇒ overlap
        /// </summary>
        private async Task<bool> HasAppointmentConflict(
            int veterinarianId,
            DateTime appointmentDateTime,
            int durationMinutes,
            int? excludeAppointmentId = null)
        {
            var start = appointmentDateTime;
            var end = appointmentDateTime.AddMinutes(durationMinutes);
            var date = appointmentDateTime.Date;

            // Base query = all appointments for same vet on same day, excluding cancelled
            var query = _context.Appointments
                .Where(a => a.VeterinarianId == veterinarianId &&
                            a.AppointmentDateTime.Date == date &&
                            a.Status != AppointmentStatus.Cancelled);

            // If editing, exclude the appointment itself from the conflict check
            if (excludeAppointmentId.HasValue)
            {
                int idToExclude = excludeAppointmentId.Value;
                query = query.Where(a => a.Id != idToExclude);
            }

            // Load existing appointments into memory
            var existing = await query
                .Select(a => new
                {
                    Start = a.AppointmentDateTime,
                    End = a.AppointmentDateTime.AddMinutes(a.DurationMinutes)
                })
                .ToListAsync();

            foreach (var item in existing)
            {
                bool overlaps = start < item.End && item.Start < end;

                if (overlaps)
                {
                    // Conflict found – no need to check the rest
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Helper – populate dropdowns for Create/Edit views.
        /// </summary>
        private async Task PopulateDropDowns(int? selectedAnimalId = null, int? selectedVetId = null)
        {
            // Load animals with owners for display
            var animals = await _context.Animals
                .Include(a => a.Owner)
                .OrderBy(a => a.Name)
                .ToListAsync();

            // Display: "AnimalName (OwnerLastName)" when owner exists
            var animalItems = animals.Select(a => new
            {
                a.Id,
                Display = a.Owner != null
                    ? $"{a.Name} ({a.Owner.LastName})"
                    : a.Name
            });

            // This is what the views use: ViewBag.Animals
            ViewBag.Animals = new SelectList(animalItems, "Id", "Display", selectedAnimalId);

            // Keep this too in case some scaffolded views still use ViewBag.AnimalId
            ViewBag.AnimalId = ViewBag.Animals;

            // Load active vets
            var vets = await _context.Veterinarians
                .Where(v => v.IsActive)
                .OrderBy(v => v.LastName)
                .ThenBy(v => v.FirstName)
                .ToListAsync();

            var vetItems = vets.Select(v => new
            {
                v.Id,
                Display = $"{v.FirstName} {v.LastName}"
            });

            // Used by the new views
            ViewBag.Veterinarians = new SelectList(vetItems, "Id", "Display", selectedVetId);

            // Keep compatibility name
            ViewBag.VeterinarianId = ViewBag.Veterinarians;
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
