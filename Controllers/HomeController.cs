using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TourFlow.Data;
using TourFlow.Models;

namespace TourFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var directions = _db.Directions
                .Where(d => d.Direction_Id >= 1 && d.Direction_Id <= 4)
                .OrderBy(d => d.Direction_Id)
                .ToList();

            var cities = await _db.Directions
                .Select(d => d.City)
                .Distinct()
                .ToListAsync();

            ViewBag.Cities = cities;
            return View(directions);
        }

        public IActionResult SearchTours()
        {
            // Логика обработки поиска
            return View();
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
}
