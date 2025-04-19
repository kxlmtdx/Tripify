using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourFlow.Data;
using TourFlow.Models;

namespace TourFlow.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult AdminPanel()
        {
            ViewBag.BookingStatusTypes = _db.BookingStatusTypes.ToList();
            var accounts = _db.Accounts.Include(a => a.AccountType).ToList();
            return View(accounts);
        }

        #region Управление турами
        public IActionResult Tours()
        {
            var tours = _db.Tours
                .Include(t => t.Direction)
                .Include(t => t.Hotel)
                .Include(t => t.TourType)
                .ToList();
            return View(tours);
        }

        public IActionResult CreateTour()
        {
            ViewBag.Directions = _db.Directions.ToList();
            ViewBag.Hotels = _db.Hotels.ToList();
            ViewBag.TourTypes = _db.ToursTypes.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTour(Tour tour)
        {
            if (ModelState.IsValid)
            {
                _db.Tours.Add(tour);
                _db.SaveChanges();
                return RedirectToAction(nameof(Tours));
            }
            return View(tour);
        }

        public IActionResult EditTour(int id)
        {
            var tour = _db.Tours.Find(id);
            ViewBag.Directions = _db.Directions.ToList();
            ViewBag.Hotels = _db.Hotels.ToList();
            ViewBag.TourTypes = _db.ToursTypes.ToList();
            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTour(Tour tour)
        {
            if (ModelState.IsValid)
            {
                _db.Tours.Update(tour);
                _db.SaveChanges();
                return RedirectToAction(nameof(Tours));
            }
            return View(tour);
        }

        [HttpPost]
        public IActionResult DeleteTour(int id)
        {
            var tour = _db.Tours.Find(id);
            _db.Tours.Remove(tour);
            _db.SaveChanges();
            return RedirectToAction(nameof(Tours));
        }
        #endregion

        #region Управление отелями
        public IActionResult Hotels()
        {
            var hotels = _db.Hotels.Include(h => h.Direction).ToList();
            return View(hotels);
        }

        public IActionResult CreateHotel()
        {
            ViewBag.Directions = _db.Directions.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateHotel(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _db.Hotels.Add(hotel);
                _db.SaveChanges();
                return RedirectToAction(nameof(Hotels));
            }
            return View(hotel);
        }

        public IActionResult EditHotel(int id)
        {
            var hotel = _db.Hotels.Find(id);
            ViewBag.Directions = _db.Directions.ToList();
            return View(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditHotel(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _db.Hotels.Update(hotel);
                _db.SaveChanges();
                return RedirectToAction(nameof(Hotels));
            }
            return View(hotel);
        }

        [HttpPost]
        public IActionResult DeleteHotel(int id)
        {
            var hotel = _db.Hotels.Find(id);
            _db.Hotels.Remove(hotel);
            _db.SaveChanges();
            return RedirectToAction(nameof(Hotels));
        }
        #endregion

        #region Управление бронированиями
        public IActionResult Bookings()
        {
            ViewBag.BookingStatusTypes = _db.BookingStatusTypes.ToList(); // Добавлено здесь

            var bookings = _db.Booking
                .Include(b => b.Account)
                .Include(b => b.BookingStatusType)
                .ToList();
            return View(bookings);
        }

        [HttpPost]
        public IActionResult UpdateBookingStatus(int bookingId, int statusId)
        {
            var booking = _db.Booking.Find(bookingId);
            if (booking != null)
            {
                booking.Booking_Status_Id = statusId;
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Bookings));
        }
        #endregion

        #region Управление документами
        public IActionResult UserDocuments()
        {
            var docs = _db.User_Documents
                .Include(d => d.Document_Type)
                .Include(d => d.Account)
                .ToList();
            return View(docs);
        }

        [HttpPost]
        public IActionResult ApproveDocument(int docId)
        {
            return RedirectToAction(nameof(UserDocuments));
        }
        #endregion
    }
}