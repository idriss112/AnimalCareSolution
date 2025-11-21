using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimalCare.Models
{
    public class VetSpecialty
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Many-to-many with Veterinarian
        public ICollection<Veterinarian> Veterinarians { get; set; } = new List<Veterinarian>();
    }
}
