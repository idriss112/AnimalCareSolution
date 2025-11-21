using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalCare.Models
{
    public class Animal
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Species { get; set; } = string.Empty; // e.g. Dog, Cat

        [StringLength(100)]
        public string? Breed { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Sex { get; set; } // "Male", "Female", etc.

        [Range(0, 500)]
        public decimal? Weight { get; set; }

        [StringLength(1000)]
        public string? ImportantNotes { get; set; }

        // Foreign key to Owner
        [Required]
        public int OwnerId { get; set; }

        // Navigation
        public Owner Owner { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: one Animal -> many Appointments
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
