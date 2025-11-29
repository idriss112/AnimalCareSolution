using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimalCare.Models
{
    /// <summary>
    /// ViewModel for creating multiple weekly schedules at once
    /// </summary>
    public class CreateVetScheduleViewModel
    {
        [Required(ErrorMessage = "Please select a veterinarian.")]
        public int VeterinarianId { get; set; }

        [Required(ErrorMessage = "Please select at least 3 working days.")]
        [MinLength(3, ErrorMessage = "Veterinarian must work at least 3 days per week.")]
        public List<int> SelectedDays { get; set; } = new List<int>();

        // Days will always be 8 AM to 4 PM (fixed)
        // No need for user input on times
    }
}