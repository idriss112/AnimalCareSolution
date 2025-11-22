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
    [Authorize]
    public class AnimalsController : Controller
    {
        private readonly AnimalCareDbContext _context;
        private readonly ILogger<AnimalsController> _logger;

        public AnimalsController(AnimalCareDbContext context, ILogger<AnimalsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Animals
        // All roles can view animals
        [Authorize(Roles = "Admin,Receptionist,Veterinarian")]
        public async Task<IActionResult> Index(string? searchTerm)
        {
            // Start query base
            var query = _context.Animals
                .Include(a => a.Owner)
                .AsQueryable();

            // Optional search by animal name or owner name
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a =>
                    a.Name.Contains(searchTerm) ||
                    a.Owner.FirstName.Contains(searchTerm) ||
                    a.Owner.LastName.Contains(searchTerm));
            }

            var animals = await query
                .OrderBy(a => a.Name)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;

            return View(animals);
        }

        // GET: Animals/Details/5
        [Authorize(Roles = "Admin,Receptionist,Veterinarian")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include Owner + Appointments
            var animal = await _context.Animals
                .Include(a => a.Owner)
                .Include(a => a.Appointments)
                    .ThenInclude(ap => ap.Veterinarian)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // GET: Animals/Create
        // Only Admin + Receptionist can create
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create()
        {
            // Dropdown of owners
            await PopulateOwnersDropDownList();
            return View();
        }

        // POST: Animals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create(
            [Bind("Name,Species,Breed,DateOfBirth,Sex,Weight,ImportantNotes,OwnerId")]
            Animal animal)
        {
            // Ensure an Owner is selected
            if (animal.OwnerId <= 0)
            {
                ModelState.AddModelError("OwnerId", "Please select an owner.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(animal);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Animal created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error creating animal");
                    TempData["ErrorMessage"] = "An error occurred while saving the animal.";
                }
            }

            // Repopulate dropdown when re-displaying form
            await PopulateOwnersDropDownList(animal.OwnerId);
            return View(animal);
        }

        // GET: Animals/Edit/5
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            await PopulateOwnersDropDownList(animal.OwnerId);
            return View(animal);
        }

        // POST: Animals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Name,Species,Breed,DateOfBirth,Sex,Weight,ImportantNotes,OwnerId")]
            Animal animal)
        {
            if (id != animal.Id)
            {
                return NotFound();
            }

            if (animal.OwnerId <= 0)
            {
                ModelState.AddModelError("OwnerId", "Please select an owner.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(animal);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Animal updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!AnimalExists(animal.Id))
                    {
                        return NotFound();
                    }

                    _logger.LogError(ex, "Concurrency error editing animal {AnimalId}", animal.Id);
                    TempData["ErrorMessage"] = "A concurrency error occurred while updating the animal.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DB error editing animal {AnimalId}", animal.Id);
                    TempData["ErrorMessage"] = "An error occurred while saving the animal.";
                }
            }

            await PopulateOwnersDropDownList(animal.OwnerId);
            return View(animal);
        }

        // GET: Animals/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Owner)
                .Include(a => a.Appointments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Appointments)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

            // Optional business rule: prevent delete if animal has appointments
            if (animal.Appointments != null && animal.Appointments.Any())
            {
                TempData["ErrorMessage"] =
                    "Cannot delete animal with appointments in the system.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Animal deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting animal {AnimalId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the animal.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper – populate Owner dropdown
        private async Task PopulateOwnersDropDownList(int? selectedOwnerId = null)
        {
            var owners = await _context.Owners
                .OrderBy(o => o.LastName)
                .ThenBy(o => o.FirstName)
                .ToListAsync();

            ViewBag.OwnerId = new SelectList(owners, "Id", "LastName", selectedOwnerId);
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }
    }
}
