using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UKCrimeWeb.Models;

namespace UKCrimeWeb.Controllers
{
    public class ProgrammeController : Controller
    {
        private readonly CrimeDbContext _context;

        public ProgrammeController(CrimeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            var programme = await _context.Programme
                .FirstOrDefaultAsync(p => p.ProgrammeId == id);

            if (programme == null)
            {
                return NotFound();
            }

            return View(programme);
        }
    }
}