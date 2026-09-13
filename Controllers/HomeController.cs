using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UKCrimeWeb.Models;

namespace UKCrimeWeb.Controllers;

public class HomeController : Controller
{
    private readonly CrimeDbContext _context;

    public HomeController(CrimeDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var featuredCase = await _context.Case
            .Where(c => c.IsFeatured)
            .OrderBy(c => c.CaseId)
            .FirstOrDefaultAsync();

        featuredCase ??= await _context.Case
            .OrderByDescending(c => c.YearStarted)
            .ThenByDescending(c => c.CaseId)
            .FirstOrDefaultAsync();

        var remainingCasesQuery = _context.Case.AsQueryable();

        if (featuredCase != null)
        {
            remainingCasesQuery = remainingCasesQuery
                .Where(c => c.CaseId != featuredCase.CaseId);
        }

        var remainingCases = await remainingCasesQuery
            .OrderByDescending(c => c.YearStarted)
            .ThenByDescending(c => c.CaseId)
            .Take(11)
            .ToListAsync();

        var cases = new List<Case>();

        if (featuredCase != null)
        {
            cases.Add(featuredCase);
        }

        cases.AddRange(remainingCases);

        return View(cases);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}