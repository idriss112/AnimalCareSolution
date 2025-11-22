using System.Collections.Generic;

namespace AnimalCare.Models
{
    public class MonthlyReportViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public int TotalAppointments { get; set; }
        public int ScheduledCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int NoShowCount { get; set; }

        public List<VetAppointmentStats> AppointmentsByVet { get; set; } = new();

        public double CancellationRate =>
            TotalAppointments > 0
                ? (double)CancelledCount / TotalAppointments * 100
                : 0;
    }

    public class VetAppointmentStats
    {
        public string VeterinarianName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
        public int CompletedCount { get; set; }
    }
}
