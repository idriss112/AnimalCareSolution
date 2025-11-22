using AnimalCare.Data;
using Microsoft.AspNetCore.Mvc;

namespace AnimalCare.Controllers
{
    public class DebugController : Controller
    {
        private readonly AnimalCareDbContext _context;

        public DebugController(AnimalCareDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var animalsCount = _context.Animals.Count();
            var ownersCount = _context.Owners.Count();
            var vetsCount = _context.Veterinarians.Count();
            

            return Content($"DB OK - Animals: {animalsCount}, Owners: {ownersCount}, Vets: {vetsCount}");
        }
    }
}
