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
        var cases = await _context.Case
            .OrderByDescending(c => c.YearStarted)
            .Take(12)
            .ToListAsync();

        return View(cases);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}