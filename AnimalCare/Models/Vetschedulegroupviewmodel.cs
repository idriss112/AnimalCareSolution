using AnimalCare.Models;

namespace AnimalCare.ViewModels
{
    public class VetScheduleGroupViewModel
    {
        public Veterinarian Veterinarian { get; set; } = default!;
        public List<VetSchedule> Schedules { get; set; } = new List<VetSchedule>();
        public int DayCount { get; set; }
    }
}














































