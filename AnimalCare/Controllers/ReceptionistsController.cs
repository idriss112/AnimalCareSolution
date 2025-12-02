using System;
using System.Linq;
using System.Threading.Tasks;
using AnimalCare.Data;
using AnimalCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimalCare.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReceptionistsController : Controller
    {
        private readonly AnimalCareDbContext _context;
        private readonly ILogger<ReceptionistsController> _logger;

        public ReceptionistsController(AnimalCareDbContext context, ILogger<ReceptionistsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Receptionists
        public async Task<IActionResult> Index()
        {
            var receptionists = await _context.Receptionists
                .OrderBy(r => r.LastName)
                .ThenBy(r => r.FirstName)
                .ToListAsync();

            return View(receptionists);
        }

        // GET: Receptionists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _context.Receptionists
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        // GET: Receptionists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Receptionists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Receptionist receptionist)
        {
            ModelState.Remove("User");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                try
                {
                    receptionist.CreatedAt = DateTime.UtcNow;
                    _context.Add(receptionist);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Receptionist created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error creating receptionist");
                    TempData["ErrorMessage"] = "An error occurred while saving the receptionist.";
                }
            }

            return View(receptionist);
        }

        // GET: Receptionists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _context.Receptionists.FindAsync(id);

            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        // POST: Receptionists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Receptionist receptionist)
        {
            if (id != receptionist.Id)
                return NotFound();

            ModelState.Remove("User");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingReceptionist = await _context.Receptionists
                        .FirstOrDefaultAsync(r => r.Id == id);

                    if (existingReceptionist == null)
                        return NotFound();

                    // Check if IsActive status changed
                    bool isActiveChanged = existingReceptionist.IsActive != receptionist.IsActive;

                    // Update properties
                    existingReceptionist.FirstName = receptionist.FirstName;
                    existingReceptionist.LastName = receptionist.LastName;
                    existingReceptionist.Email = receptionist.Email;
                    existingReceptionist.PhoneNumber = receptionist.PhoneNumber;
                    existingReceptionist.IsActive = receptionist.IsActive;

                    await _context.SaveChangesAsync();

                    // Sync IsActive status with user account
                    if (isActiveChanged)
                    {
                        var user = await _context.Users
                            .FirstOrDefaultAsync(u => u.ReceptionistId == existingReceptionist.Id);

                        if (user != null)
                        {
                            user.IsActive = receptionist.IsActive;
                            await _context.SaveChangesAsync();

                            _logger.LogInformation("Receptionist {ReceptionistId} IsActive status changed to {IsActive}. User account also updated.",
                                existingReceptionist.Id, receptionist.IsActive);
                        }
                    }

                    TempData["SuccessMessage"] = "Receptionist updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ReceptionistExists(id))
                        return NotFound();

                    _logger.LogError(ex, "Concurrency error editing receptionist {ReceptionistId}", id);
                    TempData["ErrorMessage"] = "A concurrency error occurred while updating the receptionist.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DB error editing receptionist {ReceptionistId}", id);
                    TempData["ErrorMessage"] = "An error occurred while saving the receptionist.";
                }
            }

            return View(receptionist);
        }

        // GET: Receptionists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _context.Receptionists
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        // POST: Receptionists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receptionist = await _context.Receptionists.FindAsync(id);

            if (receptionist == null)
                return NotFound();

            try
            {
                // Check if receptionist has a linked user account
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.ReceptionistId == receptionist.Id);

                if (user != null)
                {
                    // Unlink user from receptionist before deletion
                    user.ReceptionistId = null;
                    await _context.SaveChangesAsync();
                }

                _context.Receptionists.Remove(receptionist);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Receptionist deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting receptionist {ReceptionistId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the receptionist. They may have related records.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReceptionistExists(int id)
        {
            return _context.Receptionists.Any(r => r.Id == id);
        }
    }
}
