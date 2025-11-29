using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalCare.Models
{
    public class VetSchedule
    {
        public int Id { get; set; }

        [Required]
        public int VeterinarianId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0); // Default 8:00 AM

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; } = new TimeSpan(16, 0, 0); // Default 4:00 PM

        public bool IsActive { get; set; } = true;

        // Navigation
        public Veterinarian Veterinarian { get; set; } = default!;
    }
}