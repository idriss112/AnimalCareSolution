using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimalCare.Models
{
    public class Receptionist
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(50)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - One-to-One with ApplicationUser
        public ApplicationUser? User { get; set; }

        // Computed property for display
        public string FullName => $"{FirstName} {LastName}";
    }
}
