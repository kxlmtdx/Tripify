using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using TourFlow.Data;
using TourFlow.Models;
using Microsoft.AspNetCore.Authorization;

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
            if (User.Identity.IsAuthenticated)
            {
                var isAdmin = User.IsInRole("Admin");
                return RedirectToAction(isAdmin ? "AdminPanel" : "ProfilePage",
                                     isAdmin ? "Admin" : "Profile");
            }

            var users = _db.Accounts.ToList();
            return View(users);
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
        public async Task<ActionResult> Enter(string login, string password)
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

            if (user.Password != password)
            {
                return BadRequest("Неверный пароль.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Account_Type_Id == 1 ? "Admin" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (user.Account_Type_Id == 1)
            {
                return RedirectToAction("AdminPanel", "Admin");
            }
            else
            {
                return RedirectToAction("ProfilePage", "Profile");
            }
        }

        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult SignUpAction(string regLogin, string regPassword, string password)
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

            var newUser = new Account
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