using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourFlow.Data;
using TourFlow.Models;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace TourFlow.Controllers
{
    public class ToursController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ToursController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> ToursList(string country, string city, decimal? minPrice, decimal? maxPrice, int? stars)
        {
            var countries = await _db.Directions
                .Select(d => d.Country)
                .Distinct()
                .ToListAsync();

            var cities = await _db.Directions
                .Select(d => d.City)
                .Distinct()
                .ToListAsync();

            ViewBag.Countries = countries;
            ViewBag.Cities = cities;
            ViewBag.SelectedCountry = country;
            ViewBag.SelectedCity = city;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Stars = stars;

            var query = _db.Tours
             .Include(t => t.Direction)
             .Include(t => t.Hotel)
             .Include(t => t.TourType)
             .AsQueryable();

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(t => t.Direction.Country == country);
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(t => t.Direction.City == city);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(t => t.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(t => t.Price <= maxPrice.Value);
            }

            if (stars.HasValue)
            {
                query = query.Where(t => t.Hotel.Stars == stars.Value);
            }

            var tours = await query
                .OrderBy(t => t.Price)
                .ToListAsync();

            return View(tours);
        }

        public async Task<IActionResult> ToursPurchase(int id)
        {
            var tour = await _db.Tours
                .Include(t => t.Direction)
                .Include(t => t.Hotel)
                .Include(t => t.TourType)
                .FirstOrDefaultAsync(t => t.Tour_Id == id);

            if (tour == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var user = await _db.Accounts
                    .Include(a => a.User_Documents)
                    .FirstOrDefaultAsync(u => u.Login == userName);

                var passport = user?.User_Documents?.FirstOrDefault(d => d.Document_Type_Id == 1);
                ViewBag.Passport = passport;
            }

            return View(tour);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmTourPurchase(int id, string passengerName, string passportNumber)
        {
            try
            {
                var user = await _db.Accounts
                    .Include(a => a.User_Documents)
                    .FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                if (user == null) return Unauthorized();

                var tour = await _db.Tours
                    .Include(t => t.Hotel)
                    .FirstOrDefaultAsync(t => t.Tour_Id == id);
                if (tour == null)
                    return NotFound();

                var booking = new Booking
                {
                    Account_Id = user.Account_Id,
                    Tour_Id = tour.Tour_Id,
                    Hotel_Id = tour.Hotel_Id,
                    Airline_Id = 1,
                    Booking_Date = DateTime.Now,
                    Booking_Status_Id = 1
                };

                using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    _db.Booking.Add(booking);
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"Тур успешно приобретен! Номер брони: {booking.Booking_Id}";
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Ошибка базы данных: {ex.InnerException?.Message ?? ex.Message}";
                    return RedirectToAction("TourPurchase", new { id });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
                    return RedirectToAction("TourPurchase", new { id });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
                return RedirectToAction("TourPurchase", new { id });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}