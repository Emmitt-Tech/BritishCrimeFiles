using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UKCrimeWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace UKCrimeWeb.Controllers
{
    public class PeopleController : Controller
    {
        private readonly CrimeDbContext _context;

        public PeopleController(CrimeDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int? caseId, string? role)
        {
            var cases = await _context.Case
                .OrderBy(c => c.Title)
                .ToListAsync();

            var model = new CreatePersonViewModel
            {
                CaseId = caseId,
                CaseTitle = cases
                    .FirstOrDefault(c => c.CaseId == caseId)
                    ?.Title,
                Role = role,
                Cases = cases
                    .Select(c => new SelectListItem
                    {
                        Value = c.CaseId.ToString(),
                        Text = c.Title
                    })
                    .ToList()
            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePersonViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? photoPath = null;

            if (model.Photo != null && model.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "people");

                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Photo.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(stream);
                }

                photoPath = $"/images/people/{fileName}";
            }

            var person = new Person
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Nickname = model.Nickname,
                PhotoPath = photoPath
            };

            _context.Person.Add(person);
            await _context.SaveChangesAsync();

            if (model.CaseId.HasValue && !string.IsNullOrWhiteSpace(model.Role))
            {
                var casePerson = new CasePerson
                {
                    CaseId = model.CaseId.Value,
                    PersonId = person.PersonId,
                    Role = model.Role,
                    SortOrder = model.SortOrder
                };
                _context.CasePerson.Add(casePerson);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = person.PersonId });
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