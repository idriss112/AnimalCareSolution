using AnimalCare.Data;
using AnimalCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimalCare.Controllers
{
    // All actions require a logged-in user
    [Authorize]
    public class OwnersController : Controller
    {
        private readonly AnimalCareDbContext _context;
        private readonly ILogger<OwnersController> _logger;

        // DbContext and logger are injected by DI
        public OwnersController(AnimalCareDbContext context, ILogger<OwnersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Owners
        // All roles can view owners list
        [Authorize(Roles = "Admin,Receptionist,Veterinarian")]
        public async Task<IActionResult> Index(string searchString)
        {
            // Include Animals for quick count in the view
            var ownersQuery = _context.Owners
                .Include(o => o.Animals)
                .AsQueryable();

            // Filter if search string is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim().ToLower(); // Trim and convert to lowercase

                ownersQuery = ownersQuery.Where(o =>
                    o.FirstName.ToLower().Contains(searchString) ||
                    o.LastName.ToLower().Contains(searchString) ||
                    (o.FirstName + " " + o.LastName).ToLower().Contains(searchString) ||  // Full name search
                    (o.Email != null && o.Email.ToLower().Contains(searchString)) ||
                    (o.PhoneNumber != null && o.PhoneNumber.Contains(searchString))
                );
            }

            var owners = await ownersQuery
                .OrderBy(o => o.LastName)
                .ThenBy(o => o.FirstName)
                .ToListAsync();

            return View(owners);
        }

        // GET: Owners/Details/5
        // Shows one owner with their animals
        [Authorize(Roles = "Admin,Receptionist,Veterinarian")]
        public async Task<IActionResult> Details(int? id)
        {
            // If no id provided → 404
            if (id == null)
            {
                return NotFound();
            }

            // Load owner + their animals
            var owner = await _context.Owners
                .Include(o => o.Animals)
                .ThenInclude(a => a.Appointments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
        // Only Admin + Receptionist can create owners
        [Authorize(Roles = "Admin,Receptionist")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Owners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create(
            // Bind only allowed properties for security
            [Bind("FirstName,LastName,Email,PhoneNumber,Address,City,Notes")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(owner);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Owner created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error creating owner");
                    TempData["ErrorMessage"] = "An error occurred while saving the owner.";
                }
            }

            // If we reach here → validation failed or DB error → redisplay form
            return View(owner);
        }

        // GET: Owners/Edit/5
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,FirstName,LastName,Email,PhoneNumber,Address,City,Notes")] Owner owner)
        {
            if (id != owner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(owner);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Owner updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!OwnerExists(owner.Id))
                    {
                        return NotFound();
                    }

                    _logger.LogError(ex, "Concurrency error editing owner {OwnerId}", owner.Id);
                    TempData["ErrorMessage"] = "A concurrency error occurred while updating the owner.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DB error editing owner {OwnerId}", owner.Id);
                    TempData["ErrorMessage"] = "An error occurred while saving the owner.";
                }
            }

            // Validation failed → redisplay form
            return View(owner);
        }

        // GET: Owners/Delete/5
        // Only Admin can delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .Include(o => o.Animals)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _context.Owners
                .Include(o => o.Animals)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (owner == null)
            {
                return NotFound();
            }

            // BUSINESS RULE: Do not delete owner if they still have animals
            if (owner.Animals != null && owner.Animals.Any())
            {
                TempData["ErrorMessage"] =
                    "Cannot delete owner with registered animals. Please reassign or remove animals first.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Owners.Remove(owner);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Owner deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting owner {OwnerId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the owner.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper: check owner existence
        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.Id == id);
        }
    }
}
