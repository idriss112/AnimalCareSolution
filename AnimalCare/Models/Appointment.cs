using System;
using System.ComponentModel.DataAnnotations;

namespace AnimalCare.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int AnimalId { get; set; }

        [Required]
        public int VeterinarianId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDateTime { get; set; }

        [Range(1, 480)] // 1 minute to 8 hours
        public int DurationMinutes { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Animal Animal { get; set; } = default!;
        public Veterinarian Veterinarian { get; set; } = default!;
    }
}
