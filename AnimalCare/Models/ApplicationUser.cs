using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AnimalCare.Models
{
    // Custom user for Animal-Care system
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        // Link to Veterinarian entity (nullable because not all users are vets)
        [Display(Name = "Veterinarian")]
        public int? VeterinarianId { get; set; }

        [ForeignKey(nameof(VeterinarianId))]
        public virtual Veterinarian? Veterinarian { get; set; }

        // Optional but useful: account metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Receptionist")]
        public int? ReceptionistId { get; set; }
        public Receptionist? Receptionist { get; set; }
    }
}
