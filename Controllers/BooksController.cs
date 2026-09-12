using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UKCrimeWeb.Models;

namespace UKCrimeWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly CrimeDbContext _context;

        public BooksController(CrimeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.PersonBooks)
                .ThenInclude(pb => pb.Person)
                .OrderBy(b => b.Title)
                .ToListAsync();

            return View(books);
        }

        public async Task<IActionResult> Create()
        {
            var model = new BookViewModel();
            await PopulatePeople(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulatePeople(model);
                return View(model);
            }

            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                YearPublished = model.YearPublished,
                ISBN = model.ISBN,
                InPrint = model.InPrint,
                GoodreadsRating = model.GoodreadsRating,
                PurchaseUrl = model.PurchaseUrl,
                ImageUrl = model.ImageUrl
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            foreach (var personId in model.SelectedPersonIds.Distinct())
            {
                _context.PersonBook.Add(new PersonBook
                {
                    PersonId = personId,
                    BookId = book.BookId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
                .Include(b => b.PersonBooks)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            var model = new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                YearPublished = book.YearPublished,
                ISBN = book.ISBN,
                InPrint = book.InPrint,
                GoodreadsRating = book.GoodreadsRating,
                PurchaseUrl = book.PurchaseUrl,
                ImageUrl = book.ImageUrl,
                SelectedPersonIds = book.PersonBooks
                    .Select(pb => pb.PersonId)
                    .ToList()
            };

            await PopulatePeople(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulatePeople(model);
                return View(model);
            }

            var book = await _context.Books
                .Include(b => b.PersonBooks)
                .FirstOrDefaultAsync(b => b.BookId == model.BookId);

            if (book == null)
            {
                return NotFound();
            }

            book.Title = model.Title;
            book.Author = model.Author;
            book.YearPublished = model.YearPublished;
            book.ISBN = model.ISBN;
            book.InPrint = model.InPrint;
            book.GoodreadsRating = model.GoodreadsRating;
            book.PurchaseUrl = model.PurchaseUrl;
            book.ImageUrl = model.ImageUrl;

            var selectedIds = model.SelectedPersonIds.Distinct().ToHashSet();
            var existingIds = book.PersonBooks.Select(pb => pb.PersonId).ToHashSet();

            var linksToRemove = book.PersonBooks
                .Where(pb => !selectedIds.Contains(pb.PersonId))
                .ToList();

            _context.PersonBook.RemoveRange(linksToRemove);

            foreach (var personId in selectedIds.Except(existingIds))
            {
                _context.PersonBook.Add(new PersonBook
                {
                    PersonId = personId,
                    BookId = book.BookId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .Include(b => b.PersonBooks)
                .ThenInclude(pb => pb.Person)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books
                .Include(b => b.PersonBooks)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            if (book.PersonBooks.Any())
            {
                _context.PersonBook.RemoveRange(book.PersonBooks);
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulatePeople(BookViewModel model)
        {
            var people = await _context.Person
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();

            var selectedIds = model.SelectedPersonIds.ToHashSet();

            model.People = people.Select(p => new SelectListItem
            {
                Value = p.PersonId.ToString(),
                Text = $"{p.FirstName} {p.LastName}".Trim(),
                Selected = selectedIds.Contains(p.PersonId)
            }).ToList();
        }
    }
}
