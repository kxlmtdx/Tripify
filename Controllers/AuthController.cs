using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using TourFlow.Data;
using TourFlow.Models;

namespace TourFlow.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AuthController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: AuthController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        // GET: AuthController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AuthController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuthController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AuthController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AuthController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AuthController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AuthController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // Дописать по человечески
        public ActionResult Enter(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Логин и пароль не могут быть пустыми.");
            }

            var user = _db.Accounts.FirstOrDefault(u => u.Login == login);

            if (user == null)
            {
                return NotFound("Пользователь с таким логином не найден.");
            }

            return View("~/Views/Home/Index.cshtml");
        }

        public ActionResult SignUp(string regLogin, string regPassword, string password)
        {
            var user = _db.Accounts.FirstOrDefault(u => u.Login == regLogin);
            if (user != null)
            {
                return BadRequest("Пользователь с таким логином уже есть.");
            }

            if (regPassword != password)
            {
                return BadRequest("Пароли не совпадают");
            }

            var newUser = new User
            {
                Login = regLogin,
                Password = regPassword // в идеале хэшируем
            };

            _db.Accounts.Add(newUser);
            _db.SaveChanges();

            return View("~/Views/Home/Index.cshtml");
        }
    }
}