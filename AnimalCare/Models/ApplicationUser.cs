using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AnimalCare.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        // Optional link to Veterinarian
        public int? LinkedVeterinarianId { get; set; }

        public Veterinarian? Veterinarian { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
