using AnimalCare.Data;
using AnimalCare.Models;
using AnimalCare.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AnimalCare.Controllers
{

    // Only admin can create/edit/delete schedules
    
    public class VetSchedulesController : Controller
    {
        private readonly AnimalCareDbContext _context;
        private readonly ILogger<VetSchedulesController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        // Fixed working hours: 8 AM to 4 PM
        private static readonly TimeSpan WorkStartTime = new TimeSpan(8, 0, 0);   // 08:00
        private static readonly TimeSpan WorkEndTime = new TimeSpan(16, 0, 0);    // 16:00

        public VetSchedulesController(AnimalCareDbContext context, ILogger<VetSchedulesController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: VetSchedules
        // List all schedules, grouped by veterinarian
        // GET: VetSchedules
        // List all schedules, optionally filtered by vetId
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? veterinarianId)
        {
            // Get schedules with veterinarians
            var schedulesQuery = from schedule in _context.VetSchedules
                                 join vet in _context.Veterinarians on schedule.VeterinarianId equals vet.Id
                                 where schedule.IsActive
                                 select new
                                 {
                                     Schedule = schedule,
                                     Veterinarian = vet
                                 };

            // Apply filter
            if (veterinarianId.HasValue)
            {
                schedulesQuery = schedulesQuery.Where(x => x.Schedule.VeterinarianId == veterinarianId.Value);
            }

            var results = await schedulesQuery.ToListAsync();

            // Create grouped schedules using the ViewModel class
            var groupedSchedules = results
                .GroupBy(x => x.Veterinarian.Id)
                .Select(g => new VetScheduleGroupViewModel
                {
                    Veterinarian = g.First().Veterinarian,
                    Schedules = g.Select(x => x.Schedule).OrderBy(s => s.DayOfWeek).ToList(),
                    DayCount = g.Count()
                })
                .ToList();

            ViewBag.GroupedSchedules = groupedSchedules;

            // Get all schedules for the model
            var schedules = results.Select(x => x.Schedule).ToList();

            await PopulateVeterinarianDropDownList(veterinarianId);

            return View(schedules);
        }
        // GET: VetSchedules/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateVeterinarianDropDownList();
            return View(new CreateVetScheduleViewModel());
        }

        // POST: VetSchedules/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVetScheduleViewModel model)
        {
            // Validate veterinarian selected
            if (model.VeterinarianId <= 0)
            {
                ModelState.AddModelError("VeterinarianId", "Please select a veterinarian.");
            }

            // Validate at least 3 days selected
            if (model.SelectedDays == null || model.SelectedDays.Count < 3)
            {
                ModelState.AddModelError("SelectedDays", "Veterinarian must work at least 3 days per week.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if vet already has schedules
                    var existingSchedules = await _context.VetSchedules
                        .Where(s => s.VeterinarianId == model.VeterinarianId && s.IsActive)
                        .ToListAsync();

                    if (existingSchedules.Any())
                    {
                        ModelState.AddModelError(string.Empty,
                            "This veterinarian already has a schedule. Please edit or delete the existing schedule first.");
                        await PopulateVeterinarianDropDownList(model.VeterinarianId);
                        return View(model);
                    }

                    // Create a schedule for each selected day
                    foreach (var dayValue in model.SelectedDays)
                    {
                        var schedule = new VetSchedule
                        {
                            VeterinarianId = model.VeterinarianId,
                            DayOfWeek = (DayOfWeek)dayValue,
                            StartTime = WorkStartTime,  // 8:00 AM
                            EndTime = WorkEndTime,      // 4:00 PM
                            IsActive = true
                        };

                        _context.VetSchedules.Add(schedule);
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Schedule created successfully for {model.SelectedDays.Count} days.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error creating vet schedule");
                    TempData["ErrorMessage"] = "An error occurred while saving the schedule.";
                }
            }

            await PopulateVeterinarianDropDownList(model.VeterinarianId);
            return View(model);
        }

        // GET: VetSchedules/EditAll/1 (veterinarianId)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAll(int? veterinarianId)
        {
            if (veterinarianId == null)
                return NotFound();

            var veterinarian = await _context.Veterinarians.FindAsync(veterinarianId);
            if (veterinarian == null)
                return NotFound();

            // Get all active schedules for this vet
            var existingSchedules = await _context.VetSchedules
                .Where(s => s.VeterinarianId == veterinarianId && s.IsActive)
                .ToListAsync();

            var model = new EditVetScheduleViewModel
            {
                VeterinarianId = veterinarianId.Value,
                VeterinarianName = $"{veterinarian.FirstName} {veterinarian.LastName}",
                SelectedDays = existingSchedules.Select(s => (int)s.DayOfWeek).ToList(),
                OriginalDays = existingSchedules.Select(s => (int)s.DayOfWeek).ToList()
            };

            return View(model);
        }

        // POST: VetSchedules/EditAll
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAll(EditVetScheduleViewModel model)
        {
            // Validate at least 3 days selected
            if (model.SelectedDays == null || model.SelectedDays.Count < 3)
            {
                ModelState.AddModelError("SelectedDays", "Veterinarian must work at least 3 days per week.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing schedules
                    var existingSchedules = await _context.VetSchedules
                        .Where(s => s.VeterinarianId == model.VeterinarianId && s.IsActive)
                        .ToListAsync();

                    // Find days to ADD (in SelectedDays but not in OriginalDays)
                    var daysToAdd = model.SelectedDays
                        .Except(model.OriginalDays)
                        .ToList();

                    // Find days to REMOVE (in OriginalDays but not in SelectedDays)
                    var daysToRemove = model.OriginalDays
                        .Except(model.SelectedDays)
                        .ToList();

                    // Add new schedules for new days
                    foreach (var dayValue in daysToAdd)
                    {
                        var schedule = new VetSchedule
                        {
                            VeterinarianId = model.VeterinarianId,
                            DayOfWeek = (DayOfWeek)dayValue,
                            StartTime = WorkStartTime,  // 8:00 AM
                            EndTime = WorkEndTime,      // 4:00 PM
                            IsActive = true
                        };
                        _context.VetSchedules.Add(schedule);
                    }

                    // Remove schedules for removed days
                    foreach (var dayValue in daysToRemove)
                    {
                        var scheduleToRemove = existingSchedules
                            .FirstOrDefault(s => (int)s.DayOfWeek == dayValue);

                        if (scheduleToRemove != null)
                        {
                            _context.VetSchedules.Remove(scheduleToRemove);
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Schedule updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error updating vet schedule");
                    TempData["ErrorMessage"] = "An error occurred while updating the schedule.";
                }
            }

            // If we get here, reload the veterinarian name
            var vet = await _context.Veterinarians.FindAsync(model.VeterinarianId);
            if (vet != null)
            {
                model.VeterinarianName = $"{vet.FirstName} {vet.LastName}";
            }

            return View(model);
        }


        // GET: VetSchedules/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var schedule = await _context.VetSchedules
                .Include(s => s.Veterinarian)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
                return NotFound();

            // Check if deleting this day would leave vet with less than 3 days
            var vetScheduleCount = await _context.VetSchedules
                .Where(s => s.VeterinarianId == schedule.VeterinarianId && s.IsActive)
                .CountAsync();

            ViewBag.VetScheduleCount = vetScheduleCount;

            return View(schedule);
        }

        // POST: VetSchedules/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.VetSchedules.FindAsync(id);
            if (schedule == null)
                return NotFound();

            try
            {
                // Check if this would leave vet with less than 3 days
                var vetScheduleCount = await _context.VetSchedules
                    .Where(s => s.VeterinarianId == schedule.VeterinarianId && s.IsActive && s.Id != id)
                    .CountAsync();

                if (vetScheduleCount < 3)
                {
                    TempData["ErrorMessage"] = "Cannot delete this schedule. Veterinarian must work at least 3 days per week.";
                    return RedirectToAction(nameof(Index));
                }

                _context.VetSchedules.Remove(schedule);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Schedule deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting vet schedule {ScheduleId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the schedule.";
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE ALL schedules for a veterinarian
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAll(int? veterinarianId)
        {
            if (veterinarianId == null)
                return NotFound();

            var veterinarian = await _context.Veterinarians.FindAsync(veterinarianId);
            if (veterinarian == null)
                return NotFound();

            var schedules = await _context.VetSchedules
                .Where(s => s.VeterinarianId == veterinarianId && s.IsActive)
                .ToListAsync();

            ViewBag.Veterinarian = veterinarian;
            ViewBag.ScheduleCount = schedules.Count;

            return View(schedules);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllConfirmed(int veterinarianId)
        {
            try
            {
                var schedules = await _context.VetSchedules
                    .Where(s => s.VeterinarianId == veterinarianId && s.IsActive)
                    .ToListAsync();

                _context.VetSchedules.RemoveRange(schedules);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "All schedules deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting all schedules for vet {VetId}", veterinarianId);
                TempData["ErrorMessage"] = "An error occurred while deleting the schedules.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateVeterinarianDropDownList(int? selectedVetId = null)
        {
            var vets = await _context.Veterinarians
                .Where(v => v.IsActive)
                .OrderBy(v => v.LastName)
                .ThenBy(v => v.FirstName)
                .ToListAsync();

            var items = vets.Select(v => new
            {
                v.Id,
                Display = $"{v.FirstName} {v.LastName}"
            });

            ViewBag.VeterinarianId = new SelectList(items, "Id", "Display", selectedVetId);
        }

        private bool VetScheduleExists(int id)
        {
            return _context.VetSchedules.Any(e => e.Id == id);
        }

        // GET: VetSchedules/MySchedule
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> MySchedule()
        {
            // Get current logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.VeterinarianId == null)
            {
                TempData["ErrorMessage"] = "You are not linked to a veterinarian profile.";
                return RedirectToAction("Index", "Home");
            }

            // Get the veterinarian's schedules
            var schedules = await _context.VetSchedules
                .Include(s => s.Veterinarian)
                .Where(s => s.VeterinarianId == user.VeterinarianId.Value)
                .OrderBy(s => s.DayOfWeek)
                .ToListAsync();

            // Get veterinarian info
            var vet = await _context.Veterinarians
                .Include(v => v.VetSpecialties)
                .FirstOrDefaultAsync(v => v.Id == user.VeterinarianId.Value);

            ViewBag.VeterinarianName = vet != null ? $"{vet.FirstName} {vet.LastName}" : "Unknown";
            ViewBag.Specialties = vet?.VetSpecialties.Select(s => s.Name).ToList() ?? new List<string>();

            return View(schedules);
        }
    }
}