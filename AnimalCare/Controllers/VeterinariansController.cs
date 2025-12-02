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
                .Include(v => v.VetSpecialties)  // ← ADD THIS LINE to load specialties
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
        public async Task<IActionResult> Create()
        {
            await PopulateSpecialtiesCheckboxList();
            return View(new Veterinarian());
        }

        // POST: Veterinarians/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Veterinarian vet, List<int>? SelectedSpecialtyIds)
        {
            // Remove navigation properties from validation
            ModelState.Remove("Appointments");
            ModelState.Remove("VetSchedules");
            ModelState.Remove("VetSpecialties");
            ModelState.Remove("User");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                try
                {
                    vet.CreatedAt = DateTime.UtcNow;

                    // Add veterinarian first
                    _context.Add(vet);
                    await _context.SaveChangesAsync();

                    // Now add the specialties if any selected
                    if (SelectedSpecialtyIds != null && SelectedSpecialtyIds.Any())
                    {
                        var specialties = await _context.VetSpecialties
                            .Where(s => SelectedSpecialtyIds.Contains(s.Id))
                            .ToListAsync();

                        foreach (var specialty in specialties)
                        {
                            vet.VetSpecialties.Add(specialty);
                        }

                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Veterinarian created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error creating veterinarian");
                    TempData["ErrorMessage"] = "An error occurred while saving the veterinarian.";
                }
            }

            await PopulateSpecialtiesCheckboxList(SelectedSpecialtyIds);
            return View(vet);
        }

        // HELPER METHOD - Add this to your controller
        private async Task PopulateSpecialtiesCheckboxList(List<int>? selectedIds = null)
        {
            var specialties = await _context.VetSpecialties
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.Specialties = specialties;
            ViewBag.SelectedSpecialtyIds = selectedIds ?? new List<int>();
        }


        // GET: Veterinarians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var vet = await _context.Veterinarians
                .Include(v => v.VetSpecialties)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vet == null)
                return NotFound();

            // Pre-select current specialties
            var selectedSpecialtyIds = vet.VetSpecialties.Select(s => s.Id).ToList();
            await PopulateSpecialtiesCheckboxList(selectedSpecialtyIds);

            return View(vet);
        }

        // POST: Veterinarians/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Veterinarian vet, List<int>? SelectedSpecialtyIds)
        {
            if (id != vet.Id)
                return NotFound();

            // Remove navigation properties from validation
            ModelState.Remove("Appointments");
            ModelState.Remove("VetSchedules");
            ModelState.Remove("VetSpecialties");
            ModelState.Remove("User");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing vet with specialties
                    var existingVet = await _context.Veterinarians
                        .Include(v => v.VetSpecialties)
                        .FirstOrDefaultAsync(v => v.Id == id);

                    if (existingVet == null)
                        return NotFound();

                    // Check if IsActive status changed
                    bool isActiveChanged = existingVet.IsActive != vet.IsActive;

                    // Update basic properties
                    existingVet.FirstName = vet.FirstName;
                    existingVet.LastName = vet.LastName;
                    existingVet.Email = vet.Email;
                    existingVet.PhoneNumber = vet.PhoneNumber;
                    existingVet.SpecializationSummary = vet.SpecializationSummary;
                    existingVet.IsActive = vet.IsActive;

                    // Update specialties - clear and re-add
                    existingVet.VetSpecialties.Clear();

                    if (SelectedSpecialtyIds != null && SelectedSpecialtyIds.Any())
                    {
                        var specialties = await _context.VetSpecialties
                            .Where(s => SelectedSpecialtyIds.Contains(s.Id))
                            .ToListAsync();

                        foreach (var specialty in specialties)
                        {
                            existingVet.VetSpecialties.Add(specialty);
                        }
                    }

                    await _context.SaveChangesAsync();

                    // ✅ SYNC IsActive STATUS WITH USER ACCOUNT
                    if (isActiveChanged)
                    {
                        var user = await _context.Users
                            .FirstOrDefaultAsync(u => u.VeterinarianId == existingVet.Id);

                        if (user != null)
                        {
                            user.IsActive = vet.IsActive;
                            await _context.SaveChangesAsync();

                            _logger.LogInformation("Veterinarian {VetId} IsActive status changed to {IsActive}. User account also updated.",
                                existingVet.Id, vet.IsActive);
                        }
                    }

                    TempData["SuccessMessage"] = "Veterinarian updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!VeterinarianExists(id))
                        return NotFound();

                    _logger.LogError(ex, "Concurrency error editing veterinarian {VetId}", id);
                    TempData["ErrorMessage"] = "A concurrency error occurred while updating the veterinarian.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DB error editing veterinarian {VetId}", id);
                    TempData["ErrorMessage"] = "An error occurred while saving the veterinarian.";
                }
            }

            await PopulateSpecialtiesCheckboxList(SelectedSpecialtyIds);
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
