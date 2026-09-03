using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UKCrimeWeb.Models;

namespace UKCrimeWeb.Controllers
{
    public class CasesController : Controller
    {
        private readonly CrimeDbContext _context;

        public CasesController(CrimeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var caseQuery = _context.Case.AsQueryable();
            var peopleQuery = _context.Person.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                caseQuery = caseQuery.Where(c =>
                    (c.Title != null && c.Title.ToLower().Contains(search)) ||
                    (c.Location != null && c.Location.ToLower().Contains(search)) ||
                    (c.Summary != null && c.Summary.ToLower().Contains(search)) ||
                    _context.CasePerson.Any(cp =>
                        cp.CaseId == c.CaseId &&
                        cp.Person != null &&
                        (
                            (cp.Person.FirstName != null && cp.Person.FirstName.ToLower().Contains(search)) ||
                            (cp.Person.LastName != null && cp.Person.LastName.ToLower().Contains(search)) ||
                            ((cp.Person.FirstName + " " + cp.Person.LastName).ToLower().Contains(search))
                        )
                    )
                );

                peopleQuery = peopleQuery.Where(p =>
                    (p.FirstName != null && p.FirstName.ToLower().Contains(search)) ||
                    (p.LastName != null && p.LastName.ToLower().Contains(search)) ||
                    ((p.FirstName + " " + p.LastName).ToLower().Contains(search))
                );
            }

            var model = new SearchResultsViewModel
            {
                Cases = await caseQuery.OrderBy(c => c.Title).ToListAsync(),
                People = await peopleQuery.OrderBy(p => p.LastName).ToListAsync(),
                Search = search
            };

            return View(model);
        }
        public async Task<IActionResult> Details(int id)
        {
            var crimeCase = await _context.Case
                .Include(c => c.TimelineEvents)
                .FirstOrDefaultAsync(c => c.CaseId == id);

            if (crimeCase == null)
            {
                return NotFound();
            }

            var casePeople = await _context.CasePerson
                .Where(cp => cp.CaseId == id)
                .Include(cp => cp.Person)
                .OrderBy(cp => cp.Person!.LastName)
                .ThenBy(cp => cp.Person!.FirstName)
                .ToListAsync();

            var people = casePeople
                .Select(cp => cp.Person!)
                .ToList();

            var personIds = people.Select(p => p.PersonId).ToList();

            var books = await _context.PersonBook
                .Where(pb => personIds.Contains(pb.PersonId))
                .Include(pb => pb.Book)
                .Select(pb => pb.Book!)
                .Distinct()
                .OrderBy(b => b.Title)
                .ToListAsync();

            var model = new CaseDetailsViewModel
            {
                Case = crimeCase,
                People = people,
                CasePeople = casePeople,
                Books = books,
                TimelineEvents = crimeCase.TimelineEvents
                    .OrderBy(te => te.EventDate ?? DateTime.MaxValue)
                    .ThenBy(te => te.SortOrder ?? int.MaxValue)
                    .ToList()
            };

            return View(model);
        }
    }
}