using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourFlow.Data;

namespace TourFlow.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> ProfilePage()
        {
            // Получаем логин текущего пользователя из claims
            var userLogin = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userLogin))
            {
                return RedirectToAction("SignIn", "Auth");
            }

            // Получаем данные пользователя из базы данных
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
    }
}