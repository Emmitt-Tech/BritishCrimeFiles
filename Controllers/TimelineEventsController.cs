using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UKCrimeWeb.Models;

namespace UKCrimeWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TimelineEventsController : Controller
    {
        private readonly CrimeDbContext _context;

        public TimelineEventsController(CrimeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int caseId)
        {
            var crimeCase = await _context.Case.FindAsync(caseId);
            if (crimeCase == null)
            {
                return NotFound();
            }

            var events = await _context.TimelineEvent
                .Where(te => te.CaseId == caseId)
                .OrderBy(te => te.EventDate ?? DateTime.MaxValue)
                .ThenBy(te => te.SortOrder ?? int.MaxValue)
                .ThenBy(te => te.Id)
                .ToListAsync();

            return View(new TimelineEditorViewModel
            {
                Case = crimeCase,
                Events = events,
                NewEvent = new TimelineEvent { CaseId = caseId }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int caseId, string displayDate, string title, string? description)
        {
            var crimeCase = await _context.Case.FindAsync(caseId);
            if (crimeCase == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(displayDate))
            {
                ModelState.AddModelError(nameof(displayDate), "Enter a date or date label.");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError(nameof(title), "Enter a timeline title.");
            }

            if (!TryParseFlexibleDate(displayDate, out var eventDate))
            {
                ModelState.AddModelError(nameof(displayDate), "Use a recognisable date such as 14 Dec 2021, Dec 2021, or 2021.");
            }

            if (!ModelState.IsValid)
            {
                var events = await _context.TimelineEvent
                    .Where(te => te.CaseId == caseId)
                    .OrderBy(te => te.EventDate ?? DateTime.MaxValue)
                    .ThenBy(te => te.SortOrder ?? int.MaxValue)
                    .ThenBy(te => te.Id)
                    .ToListAsync();

                return View("Index", new TimelineEditorViewModel
                {
                    Case = crimeCase,
                    Events = events,
                    NewEvent = new TimelineEvent
                    {
                        CaseId = caseId,
                        DisplayDate = displayDate,
                        Title = title,
                        Description = description
                    }
                });
            }

            var nextSortOrder = (await _context.TimelineEvent
                .Where(te => te.CaseId == caseId && te.EventDate == eventDate)
                .MaxAsync(te => (int?)te.SortOrder) ?? 0) + 1;

            _context.TimelineEvent.Add(new TimelineEvent
            {
                CaseId = caseId,
                EventDate = eventDate,
                DisplayDate = displayDate.Trim(),
                Title = title.Trim(),
                Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                SortOrder = nextSortOrder
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { caseId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var timelineEvent = await _context.TimelineEvent
                .Include(te => te.Case)
                .FirstOrDefaultAsync(te => te.Id == id);

            if (timelineEvent == null)
            {
                return NotFound();
            }

            return View(timelineEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int caseId, string displayDate, string title, string? description)
        {
            var timelineEvent = await _context.TimelineEvent.FindAsync(id);
            if (timelineEvent == null || timelineEvent.CaseId != caseId)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(displayDate))
            {
                ModelState.AddModelError(nameof(displayDate), "Enter a date or date label.");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError(nameof(title), "Enter a timeline title.");
            }

            if (!TryParseFlexibleDate(displayDate, out var eventDate))
            {
                ModelState.AddModelError(nameof(displayDate), "Use a recognisable date such as 14 Dec 2021, Dec 2021, or 2021.");
            }

            if (!ModelState.IsValid)
            {
                timelineEvent.DisplayDate = displayDate;
                timelineEvent.Title = title;
                timelineEvent.Description = description;
                timelineEvent.Case = await _context.Case.FindAsync(caseId);
                return View(timelineEvent);
            }

            timelineEvent.EventDate = eventDate;
            timelineEvent.DisplayDate = displayDate.Trim();
            timelineEvent.Title = title.Trim();
            timelineEvent.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { caseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var timelineEvent = await _context.TimelineEvent.FindAsync(id);
            if (timelineEvent == null)
            {
                return NotFound();
            }

            var caseId = timelineEvent.CaseId;
            _context.TimelineEvent.Remove(timelineEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { caseId });
        }

        private static bool TryParseFlexibleDate(string? input, out DateTime date)
        {
            date = default;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var value = input.Trim();
            var culture = CultureInfo.GetCultureInfo("en-GB");

            var exactFormats = new[]
            {
                "d MMM yyyy",
                "dd MMM yyyy",
                "d MMMM yyyy",
                "dd MMMM yyyy",
                "MMM yyyy",
                "MMMM yyyy",
                "yyyy"
            };

            if (DateTime.TryParseExact(value, exactFormats, culture,
                    DateTimeStyles.AllowWhiteSpaces, out date))
            {
                return true;
            }

            return DateTime.TryParse(value, culture, DateTimeStyles.AllowWhiteSpaces, out date);
        }
    }
}
