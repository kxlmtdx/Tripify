using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourFlow.Data;
using TourFlow.Models;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace TourFlow.Controllers
{
    public class FlightsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FlightsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> FlightsList(string from, string to, string date)
        {
            var cities = await _db.Directions
                .Select(d => d.City)
                .Distinct()
                .ToListAsync();

            ViewBag.Cities = cities;
            ViewBag.SelectedFrom = from;
            ViewBag.SelectedTo = to;
            ViewBag.SelectedDate = date;

            if (string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to) && string.IsNullOrEmpty(date))
            {
                return View(new List<FlightTicket>());
            }

            var query = _db.Flight_Tickets
                .Include(ft => ft.Airline)
                .Include(ft => ft.FlightType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(from))
            {
                query = query.Where(ft => ft.Departure_City.Contains(from));
            }

            if (!string.IsNullOrEmpty(to))
            {
                query = query.Where(ft => ft.Arrival_City.Contains(to));
            }

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime departureDate))
            {
                query = query.Where(ft => ft.Departure_Date.Date == departureDate.Date);
            }

            var tickets = await query
                .OrderBy(ft => ft.Price)
                .ToListAsync();

            return View(tickets);
        }

        public async Task<IActionResult> FlightPurchase(int id)
        {
            var ticket = await _db.Flight_Tickets
                .Include(ft => ft.Airline)
                .Include(ft => ft.FlightType)
                .FirstOrDefaultAsync(ft => ft.Ticket_Id == id);

            if (ticket == null)
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

            return View(ticket);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPurchase(int id, string passengerName, string passportNumber)
        {
            try
            {
                var user = await _db.Accounts
                    .Include(a => a.User_Documents)
                    .FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                if (user == null) return Unauthorized();

                var ticket = await _db.Flight_Tickets
                    .FirstOrDefaultAsync(ft => ft.Ticket_Id == id);
                if (ticket == null)
                    return NotFound();

                if (await _db.Tours.AllAsync(t => t.Tour_Id != 1))
                {
                    TempData["ErrorMessage"] = "Необходимо указать действительный тур";
                    return RedirectToAction("Index", "Home");
                }

                var booking = new Booking
                {
                    Account_Id = user.Account_Id,
                    Airline_Id = ticket.Airline_Id,
                    Booking_Date = DateTime.Now,
                    Booking_Status_Id = 1,
                    Tour_Id = 1,
                    Account = user
                };

                using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    _db.Booking.Add(booking);
                    await _db.SaveChangesAsync();

                    ticket.Booking_Id = booking.Booking_Id;
                    await _db.SaveChangesAsync();

                    await transaction.CommitAsync();
                    //TempData["SuccessMessage"] = $"Билет успешно приобретен! Номер: {booking.Booking_Id}";
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Ошибка базы данных: {ex.InnerException?.Message ?? ex.Message}";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}