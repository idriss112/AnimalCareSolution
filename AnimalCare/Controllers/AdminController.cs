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
            // Get counts for dashboard
            ViewBag.OwnersCount = await _context.Owners.CountAsync();
            ViewBag.AnimalsCount = await _context.Animals.CountAsync();
            ViewBag.VeterinariansCount = await _context.Veterinarians.CountAsync();
            ViewBag.ReceptionistsCount = await _context.Receptionists.CountAsync(); // ← ADD THIS LINE
            ViewBag.AppointmentsCount = await _context.Appointments.CountAsync();

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

            _logger.LogInformation("Monthly Report: Filtering appointments from {StartDate} to {EndDate}",
                startDate, endDate);

            // Load appointments in this time range
            var appointments = await _context.Appointments
                .Include(a => a.Veterinarian)
                .Where(a => a.AppointmentDateTime >= startDate &&
                            a.AppointmentDateTime < endDate)
                .ToListAsync();

            _logger.LogInformation("Found {Count} appointments for {Month}/{Year}",
                appointments.Count, reportMonth, reportYear);

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
                .GroupBy(a => a.Veterinarian!.Id)
                .Select(g => new VetAppointmentStats
                {
                    VeterinarianName = $"{g.First().Veterinarian!.FirstName} {g.First().Veterinarian!.LastName}",
                    AppointmentCount = g.Count(),
                    CompletedCount = g.Count(a => a.Status == AppointmentStatus.Completed)
                })
                .OrderByDescending(v => v.AppointmentCount)
                .ToList();

            _logger.LogInformation("Report generated with {VetCount} veterinarians",
                report.AppointmentsByVet.Count);

            return View(report);
        }
    }
}
