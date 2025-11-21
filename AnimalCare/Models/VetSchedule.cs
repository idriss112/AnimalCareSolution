using System;
using System.ComponentModel.DataAnnotations;

namespace AnimalCare.Models
{
    public class VetSchedule
    {
        public int Id { get; set; }

        [Required]
        public int VeterinarianId { get; set; }

        // Optional specific date for special schedule
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        // Optional recurring weekly schedule (e.g. every Monday)
        public DayOfWeek? DayOfWeek { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        public bool IsRecurring { get; set; } = false;

        public bool IsActive { get; set; } = true;

        // Navigation
        public Veterinarian Veterinarian { get; set; } = default!;
    }
}
