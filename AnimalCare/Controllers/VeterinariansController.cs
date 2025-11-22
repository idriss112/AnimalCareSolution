using AnimalCare.Data;
using AnimalCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimalCare.Controllers
{
    // Only Admin can manage veterinarians
    [Authorize(Roles = "Admin")]
    public class VeterinariansController : Controller
    {
        private readonly AnimalCareDbContext _context;
        private readonly ILogger<VeterinariansController> _logger;

        public VeterinariansController(AnimalCareDbContext context, ILogger<VeterinariansController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Veterinarians
        // List all vets with IsActive flag and basic info
        public async Task<IActionResult> Index()
        {
            var vets = await _context.Veterinarians
                .Include(v => v.VetSchedules)    // for schedule count, if needed in view
                .Include(v => v.Appointments)    // for appointment count
                .OrderBy(v => v.LastName)
                .ThenBy(v => v.FirstName)
                .ToListAsync();

            return View(vets);
        }

        // GET: Veterinarians/Details/5
        // Show vet details, schedules and basic appointment stats
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var vet = await _context.Veterinarians
                .Include(v => v.VetSchedules)
                .Include(v => v.Appointments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vet == null)
                return NotFound();

            // You can compute stats and pass via ViewBag if needed
            var futureAppointmentsCount = vet.Appointments
                .Count(a => a.AppointmentDateTime >= DateTime.Now);

            ViewBag.FutureAppointmentsCount = futureAppointmentsCount;

            return View(vet);
        }

        // GET: Veterinarians/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Veterinarians/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FirstName,LastName,Email,PhoneNumber,SpecializationSummary,IsActive")]
            Veterinarian vet)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(vet);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Veterinarian created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error creating veterinarian");
                    TempData["ErrorMessage"] = "An error occurred while saving the veterinarian.";
                }
            }

            return View(vet);
        }

        // GET: Veterinarians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var vet = await _context.Veterinarians.FindAsync(id);
            if (vet == null)
                return NotFound();

            return View(vet);
        }

        // POST: Veterinarians/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,FirstName,LastName,Email,PhoneNumber,SpecializationSummary,IsActive")]
            Veterinarian vet)
        {
            if (id != vet.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vet);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Veterinarian updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!VeterinarianExists(vet.Id))
                        return NotFound();

                    _logger.LogError(ex, "Concurrency error editing veterinarian {VetId}", vet.Id);
                    TempData["ErrorMessage"] = "A concurrency error occurred while updating the veterinarian.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DB error editing veterinarian {VetId}", vet.Id);
                    TempData["ErrorMessage"] = "An error occurred while saving the veterinarian.";
                }
            }

            return View(vet);
        }

        // GET: Veterinarians/Delete/5
        // We will WARN if vet has future appointments and recommend deactivation instead.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var vet = await _context.Veterinarians
                .Include(v => v.Appointments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (vet == null)
                return NotFound();

            var hasFutureAppointments = vet.Appointments
                .Any(a => a.AppointmentDateTime > DateTime.Now &&
                          a.Status != AppointmentStatus.Cancelled);

            ViewBag.HasFutureAppointments = hasFutureAppointments;

            return View(vet);
        }

        // POST: Veterinarians/Delete/5
        // Business rule:
        // - if vet has future appointments, we block deletion and suggest setting IsActive = false instead
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vet = await _context.Veterinarians
                .Include(v => v.Appointments)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vet == null)
                return NotFound();

            if (HasFutureAppointments(vet.Id))
            {
                TempData["ErrorMessage"] =
                    "Cannot delete veterinarian with future appointments. Set them as inactive instead.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Veterinarians.Remove(vet);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Veterinarian deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting veterinarian {VetId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the veterinarian.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VeterinarianExists(int id)
        {
            return _context.Veterinarians.Any(e => e.Id == id);
        }

        /// <summary>
        /// Check if a veterinarian has any non-cancelled appointments in the future.
        /// Used to decide if deletion is allowed.
        /// </summary>
        private bool HasFutureAppointments(int veterinarianId)
        {
            var now = DateTime.Now;

            return _context.Appointments.Any(a =>
                a.VeterinarianId == veterinarianId &&
                a.AppointmentDateTime > now &&
                a.Status != AppointmentStatus.Cancelled);
        }
    }
}
