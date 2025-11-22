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
    // Admin-only controller
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AnimalCareDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AnimalCareDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Simple dashboard page
        public async Task<IActionResult> Dashboard()
        {
            // Quick overview – count of entities
            var totalAppointments = await _context.Appointments.CountAsync();
            var totalAnimals = await _context.Animals.CountAsync();
            var totalOwners = await _context.Owners.CountAsync();
            var totalVets = await _context.Veterinarians.CountAsync();

            ViewBag.TotalAppointments = totalAppointments;
            ViewBag.TotalAnimals = totalAnimals;
            ViewBag.TotalOwners = totalOwners;
            ViewBag.TotalVets = totalVets;

            return View();
        }

        // Monthly report: Admin/MonthlyReport?month=11&year=2025
        public async Task<IActionResult> MonthlyReport(int? month, int? year)
        {
            // Default to current month if not provided
            int reportMonth = month ?? DateTime.Now.Month;
            int reportYear = year ?? DateTime.Now.Year;

            // Calculate date range for month
            var startDate = new DateTime(reportYear, reportMonth, 1);
            var endDate = startDate.AddMonths(1); // exclusive upper bound

            // Load appointments in this time range
            var appointments = await _context.Appointments
                .Include(a => a.Veterinarian)
                .Where(a => a.AppointmentDateTime >= startDate &&
                            a.AppointmentDateTime < endDate)
                .ToListAsync();

            var report = new MonthlyReportViewModel
            {
                Month = reportMonth,
                Year = reportYear,
                TotalAppointments = appointments.Count,
                ScheduledCount = appointments.Count(a => a.Status == AppointmentStatus.Scheduled),
                CompletedCount = appointments.Count(a => a.Status == AppointmentStatus.Completed),
                CancelledCount = appointments.Count(a => a.Status == AppointmentStatus.Cancelled),
                NoShowCount = appointments.Count(a => a.Status == AppointmentStatus.NoShow)
            };

            // Group by veterinarian to compute per-vet stats
            report.AppointmentsByVet = appointments
                .Where(a => a.Veterinarian != null)
                .GroupBy(a => a.Veterinarian)
                .Select(g => new VetAppointmentStats
                {
                    VeterinarianName = $"{g.Key.FirstName} {g.Key.LastName}",
                    AppointmentCount = g.Count(),
                    CompletedCount = g.Count(a => a.Status == AppointmentStatus.Completed)
                })
                .OrderByDescending(v => v.AppointmentCount)
                .ToList();

            return View(report);
        }
    }
}
