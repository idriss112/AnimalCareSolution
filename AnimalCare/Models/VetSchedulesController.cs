using System;
using System.Linq;
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
    // Only admin can create/edit/delete schedules
    [Authorize(Roles = "Admin")]
    public class VetSchedulesController : Controller
    {
        private readonly AnimalCareDbContext _context;
        private readonly ILogger<VetSchedulesController> _logger;

        // Define clinic opening hours (you can adjust)
        private static readonly TimeSpan ClinicOpenTime = new TimeSpan(8, 0, 0);   // 08:00
        private static readonly TimeSpan ClinicCloseTime = new TimeSpan(20, 0, 0); // 20:00

        public VetSchedulesController(AnimalCareDbContext context, ILogger<VetSchedulesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: VetSchedules
        // List all schedules, optionally filtered by vetId
        public async Task<IActionResult> Index(int? veterinarianId)
        {
            var query = _context.VetSchedules
                .Include(s => s.Veterinarian)
                .OrderBy(s => s.Veterinarian.LastName)
                .ThenBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .AsQueryable();

            if (veterinarianId.HasValue)
            {
                query = query.Where(s => s.VeterinarianId == veterinarianId.Value);
            }

            var schedules = await query.ToListAsync();

            await PopulateVeterinarianDropDownList(veterinarianId);

            return View(schedules);
        }

        // GET: VetSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var schedule = await _context.VetSchedules
                .Include(s => s.Veterinarian)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
                return NotFound();

            return View(schedule);
        }

        // GET: VetSchedules/Create
        public async Task<IActionResult> Create()
        {
            await PopulateVeterinarianDropDownList();
            return View();
        }

        // POST: VetSchedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("VeterinarianId,Date,StartTime,EndTime,IsActive")]
            VetSchedule schedule)
        {
            // Validate vet selected
            if (schedule.VeterinarianId <= 0)
            {
                ModelState.AddModelError("VeterinarianId", "Please select a veterinarian.");
            }

            // Validate times
            ValidateScheduleTimes(schedule);

            if (ModelState.IsValid)
            {
                // Check overlap for same vet, same date
                bool hasConflict = await HasScheduleConflict(
                    schedule.VeterinarianId,
                    schedule.Date.Value.Date,
                    schedule.StartTime,
                    schedule.EndTime,
                    null);

                if (hasConflict)
                {
                    ModelState.AddModelError(string.Empty,
                        "This schedule overlaps with an existing schedule for this veterinarian.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(schedule);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Schedule created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error creating vet schedule");
                    TempData["ErrorMessage"] = "An error occurred while saving the schedule.";
                }
            }

            await PopulateVeterinarianDropDownList(schedule.VeterinarianId);
            return View(schedule);
        }

        // GET: VetSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var schedule = await _context.VetSchedules.FindAsync(id);
            if (schedule == null)
                return NotFound();

            await PopulateVeterinarianDropDownList(schedule.VeterinarianId);
            return View(schedule);
        }

        // POST: VetSchedules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,VeterinarianId,Date,StartTime,EndTime,IsActive")]
            VetSchedule schedule)
        {
            if (id != schedule.Id)
                return NotFound();

            if (schedule.VeterinarianId <= 0)
            {
                ModelState.AddModelError("VeterinarianId", "Please select a veterinarian.");
            }

            ValidateScheduleTimes(schedule);

            if (ModelState.IsValid)
            {
                bool hasConflict = await HasScheduleConflict(
                    schedule.VeterinarianId,
                    schedule.Date.Value.Date,
                    schedule.StartTime,
                    schedule.EndTime,
                    schedule.Id);

                if (hasConflict)
                {
                    ModelState.AddModelError(string.Empty,
                        "This schedule overlaps with another schedule for this veterinarian.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Schedule updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!VetScheduleExists(schedule.Id))
                        return NotFound();

                    _logger.LogError(ex, "Concurrency error editing vet schedule {ScheduleId}", schedule.Id);
                    TempData["ErrorMessage"] = "A concurrency error occurred while updating the schedule.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DB error editing vet schedule {ScheduleId}", schedule.Id);
                    TempData["ErrorMessage"] = "An error occurred while saving the schedule.";
                }
            }

            await PopulateVeterinarianDropDownList(schedule.VeterinarianId);
            return View(schedule);
        }

        // GET: VetSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var schedule = await _context.VetSchedules
                .Include(s => s.Veterinarian)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
                return NotFound();

            return View(schedule);
        }

        // POST: VetSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.VetSchedules.FindAsync(id);
            if (schedule == null)
                return NotFound();

            try
            {
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

        // Helper: validate business rules for time range + clinic hours
        private void ValidateScheduleTimes(VetSchedule schedule)
        {
            // Start must be before End
            if (schedule.StartTime >= schedule.EndTime)
            {
                ModelState.AddModelError("StartTime", "Start time must be before end time.");
            }

            // Within clinic hours
            if (schedule.StartTime < ClinicOpenTime || schedule.EndTime > ClinicCloseTime)
            {
                ModelState.AddModelError(string.Empty,
                    $"Schedule must be within clinic hours ({ClinicOpenTime} - {ClinicCloseTime}).");
            }
        }

        /// <summary>
        /// Check for overlap with existing schedules for the same vet on the same date.
        /// Overlap rule for schedules:
        /// (Start1 < End2) AND (Start2 < End1)
        /// </summary>
        private async Task<bool> HasScheduleConflict(
            int veterinarianId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeScheduleId = null)
        {
            var query = _context.VetSchedules
                .Where(s => s.VeterinarianId == veterinarianId &&
                            s.Date.Value.Date == date.Date);

            if (excludeScheduleId.HasValue)
            {
                int idToExclude = excludeScheduleId.Value;
                query = query.Where(s => s.Id != idToExclude);
            }

            var existingSchedules = await query
                .Select(s => new
                {
                    s.StartTime,
                    s.EndTime
                })
                .ToListAsync();

            foreach (var s in existingSchedules)
            {
                bool overlaps = startTime < s.EndTime && s.StartTime < endTime;

                if (overlaps)
                {
                    return true;
                }
            }

            return false;
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
    }
}
