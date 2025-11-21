using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimalCare.Models
{
    public class Veterinarian
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? SpecializationSummary { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: one Vet -> many Appointments
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        // Navigation: one Vet -> many Schedules
        public ICollection<VetSchedule> VetSchedules { get; set; } = new List<VetSchedule>();

        // Navigation: many-to-many with VetSpecialty
        public ICollection<VetSpecialty> VetSpecialties { get; set; } = new List<VetSpecialty>();

        // Navigation: optional link to ApplicationUser
        public ApplicationUser? User { get; set; }
    }
}
