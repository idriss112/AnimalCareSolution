using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimalCare.Models
{
    /// <summary>
    /// ViewModel for editing all schedules for a veterinarian
    /// </summary>
    public class EditVetScheduleViewModel
    {
        [Required]
        public int VeterinarianId { get; set; }

        public string VeterinarianName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select at least 3 working days.")]
        [MinLength(3, ErrorMessage = "Veterinarian must work at least 3 days per week.")]
        public List<int> SelectedDays { get; set; } = new List<int>();

        // For tracking which days were originally selected
        public List<int> OriginalDays { get; set; } = new List<int>();
    }
}