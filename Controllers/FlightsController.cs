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
                var departureDirection = await _db.Directions
                    .FirstOrDefaultAsync(d => d.City == from);

                if (departureDirection != null)
                {
                    query = query.Where(ft => ft.Departure_Direction_Id == departureDirection.Direction_Id);
                }
            }

            if (!string.IsNullOrEmpty(to))
            {
                var arrivalDirection = await _db.Directions
                    .FirstOrDefaultAsync(d => d.City == to);

                if (arrivalDirection != null)
                {
                    query = query.Where(ft => ft.Arrival_Direction_Id == arrivalDirection.Direction_Id);
                }
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

            var departureDirection = await _db.Directions
                .FirstOrDefaultAsync(d => d.Direction_Id == ticket.Departure_Direction_Id);
            var arrivalDirection = await _db.Directions
                .FirstOrDefaultAsync(d => d.Direction_Id == ticket.Arrival_Direction_Id);

            var availableHotels = await _db.Hotels
                .Include(h => h.Direction)
                .Where(h => h.Direction.City == arrivalDirection.City)
                .ToListAsync();

            ViewBag.DepartureCity = departureDirection?.City;
            ViewBag.ArrivalCity = arrivalDirection?.City;
            ViewBag.DepartureAirportCode = GetAirportCode(departureDirection?.City);
            ViewBag.ArrivalAirportCode = GetAirportCode(arrivalDirection?.City);
            ViewBag.AvailableHotels = availableHotels;

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
        public async Task<IActionResult> ConfirmPurchase(int id, string passengerName, string passportNumber,
            bool includeHotel = false, int? selectedHotelId = null)
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

                Hotel hotel = null;
                if (includeHotel)
                {
                    if (!selectedHotelId.HasValue)
                    {
                        TempData["ErrorMessage"] = "Необходимо выбрать отель";
                        return RedirectToAction("FlightPurchase", new { id });
                    }

                    hotel = await _db.Hotels
                        .FirstOrDefaultAsync(h => h.Hotel_Id == selectedHotelId.Value);

                    if (hotel == null)
                    {
                        TempData["ErrorMessage"] = "Выбранный отель не найден";
                        return RedirectToAction("FlightPurchase", new { id });
                    }
                }

                var booking = new Booking
                {
                    Account_Id = user.Account_Id,
                    Airline_Id = ticket.Airline_Id,
                    Booking_Date = DateTime.Now,
                    Booking_Status_Id = 1,
                    Hotel_Id = includeHotel ? selectedHotelId : null
                };

                using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    _db.Booking.Add(booking);
                    await _db.SaveChangesAsync();

                    ticket.Booking_Id = booking.Booking_Id;
                    _db.Flight_Tickets.Update(ticket);
                    await _db.SaveChangesAsync();

                    await transaction.CommitAsync();

                    var successMessage = "Билет успешно приобретен!";
                    if (includeHotel && hotel != null)
                    {
                        successMessage += $" Отель \"{hotel.Name}\" добавлен в заказ.";
                    }
                    successMessage += $" Номер брони: {booking.Booking_Id}";

                    TempData["SuccessMessage"] = successMessage;
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Ошибка базы данных: {ex.InnerException?.Message ?? ex.Message}";
                    return RedirectToAction("FlightPurchase", new { id });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
                    return RedirectToAction("FlightPurchase", new { id });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
                return RedirectToAction("FlightPurchase", new { id });
            }

            return RedirectToAction("Index", "Home");
        }

        public string GetAirportCode(string city)
        {
            if (string.IsNullOrEmpty(city)) return "UNK";

            return city switch
            {
                "Москва" => "SVO",
                "Санкт-Петербург" => "LED",
                "Абакан" => "ABA",
                "Домодедово" => "DME",
                _ => city.Length >= 3 ? city.Substring(0, 3).ToUpper() : city.ToUpper()
            };
        }
    }
}