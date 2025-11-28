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
        // Everyone logged in can view list
        public async Task<IActionResult> Index(string searchString)
        {
            var animalsQuery = _context.Animals
                .Include(a => a.Owner)
                .AsQueryable();

            // Filter if search string is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();

                animalsQuery = animalsQuery.Where(a =>
                    a.Name.ToLower().Contains(searchString) ||
                    a.Species.ToLower().Contains(searchString) ||
                    (a.Breed != null && a.Breed.ToLower().Contains(searchString)) ||
                    (a.Owner != null && (
                        a.Owner.FirstName.ToLower().Contains(searchString) ||
                        a.Owner.LastName.ToLower().Contains(searchString)
                    ))
                );
            }

            var animals = await animalsQuery
                .OrderBy(a => a.Name)
                .ToListAsync();

            return View(animals);
        }

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animals
                .Include(a => a.Owner)
                .Include(a => a.Appointments)
                .ThenInclude(ap => ap.Veterinarian)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null) return NotFound();

            return View(animal);
        }

        // GET: Animals/Create
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("GET Create called");
            await PopulateOwnerDropDown();
            return View();
        }

        // POST: Animals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create(Animal animal)
        {
            _logger.LogInformation("POST Create called");
            _logger.LogInformation($"Animal Name: {animal.Name}");
            _logger.LogInformation($"Animal Species: {animal.Species}");
            _logger.LogInformation($"Animal OwnerId: {animal.OwnerId}");

            // Remove CreatedAt from model state as it's set automatically
            ModelState.Remove("CreatedAt");

            // Remove Owner navigation property from validation
            ModelState.Remove("Owner");

            // Remove Appointments from validation
            ModelState.Remove("Appointments");

            if (animal.OwnerId <= 0)
            {
                ModelState.AddModelError("OwnerId", "Please select an owner.");
                _logger.LogWarning("OwnerId is 0 or less");
            }

            // Log all model state errors
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    animal.CreatedAt = DateTime.UtcNow;
                    _context.Animals.Add(animal);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Animal created successfully with ID: {animal.Id}");
                    TempData["SuccessMessage"] = "Animal created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error creating animal");
                    TempData["ErrorMessage"] = "An error occurred while saving the animal.";
                }
            }

            _logger.LogInformation("Returning to Create view with errors");
            await PopulateOwnerDropDown(animal.OwnerId);
            return View(animal);
        }

        // GET: Animals/Edit/5
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

            await PopulateOwnerDropDown(animal.OwnerId);
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
            if (id != animal.Id) return NotFound();

            // Remove navigation properties from validation
            ModelState.Remove("Owner");
            ModelState.Remove("Appointments");
            ModelState.Remove("CreatedAt");

           

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
                    TempData["ErrorMessage"] = "A concurrency error occurred while saving the animal.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error editing animal {AnimalId}", animal.Id);
                    TempData["ErrorMessage"] = "An error occurred while saving the animal.";
                }
            }

            await PopulateOwnerDropDown(animal.OwnerId);
            return View(animal);
        }

        // GET: Animals/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animals
                .Include(a => a.Owner)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null) return NotFound();

            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

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

        // ---------- Helpers ----------

        private async Task PopulateOwnerDropDown(int? selectedOwnerId = null)
        {
            var owners = await _context.Owners
                .OrderBy(o => o.LastName)
                .ThenBy(o => o.FirstName)
                .Select(o => new
                {
                    o.Id,
                    FullName = o.FirstName + " " + o.LastName
                })
                .ToListAsync();

            ViewBag.OwnerId = new SelectList(owners, "Id", "FullName", selectedOwnerId);
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }
    }
}