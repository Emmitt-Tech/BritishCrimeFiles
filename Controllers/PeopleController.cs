using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UKCrimeWeb.Models;

namespace UKCrimeWeb.Controllers
{
    public class PeopleController : Controller
    {
        private readonly CrimeDbContext _context;

        public PeopleController(CrimeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            var person = await _context.Person
                .FirstOrDefaultAsync(p => p.PersonId == id);

            if (person == null)
            {
                return NotFound();
            }

            var programmes = await _context.PersonProgramme
                .Where(pp => pp.PersonId == id)
                .Include(pp => pp.Programme)
                .Select(pp => pp.Programme!)
                .OrderBy(p => p.BroadcastYear)
                .ThenBy(p => p.Title)
                .ToListAsync();

            var books = await _context.PersonBook
                .Where(pb => pb.PersonId == id)
                .Include(pb => pb.Book)
                .Select(pb => pb.Book!)
                .OrderBy(b => b.Title)
                .ToListAsync();

            var model = new PersonDetailsViewModel
            {
                Person = person,
                Programmes = programmes,
                Books = books
            };

            return View(model);
        }
    }
}