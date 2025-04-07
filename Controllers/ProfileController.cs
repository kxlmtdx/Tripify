using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourFlow.Data;
using TourFlow.Models;
using Microsoft.AspNetCore.Authorization;

namespace TourFlow.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> ProfilePage()
        {
            var userLogin = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userLogin))
            {
                return RedirectToAction("SignIn", "Auth");
            }

            var user = await _context.Accounts
                .Include(a => a.User_Documents)
                .FirstOrDefaultAsync(a => a.Login == userLogin);

            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            var hasPassport = user.User_Documents.Any(d => d.Document_Type_Id == 1); // паспорт
            var hasInternationalPassport = user.User_Documents.Any(d => d.Document_Type_Id == 2); // загран

            ViewBag.Documents = new
            {
                HasPassport = hasPassport,
                HasInternationalPassport = hasInternationalPassport
            };

            return View(user);
        }

        [HttpGet]
        public async Task<ActionResult> EditProfile()
        {
            var userLogin = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userLogin))
            {
                return RedirectToAction("SignIn", "Auth");
            }

            var user = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Login == userLogin);

            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(Account model)
        {
            // костылим
            ModelState.Remove("Login");
            ModelState.Remove("Password");
            ModelState.Remove("AccountType");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userLogin = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Accounts.FirstOrDefaultAsync(a => a.Login == userLogin);

            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            // костылим
            model.Login = user.Login;
            model.Password = user.Password;
            model.AccountType = user.AccountType;

            user.fName = model.fName;
            user.Email = model.Email;

            try
            {
                _context.Accounts.Update(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Профиль успешно обновлен";
                return RedirectToAction("ProfilePage");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при обновлении профиля: " + ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> ExitProfile()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddDocuments()
        {
            return View();
        }
    }
}