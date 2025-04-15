using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourFlow.Data;
using TourFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

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
            var hasInternationalPassport = user.User_Documents.Any(d => d.Document_Type_Id == 4); // загран

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

        public async Task<ActionResult> ProfileDocuments()
        {
            var userLogin = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Accounts
                .Include(a => a.User_Documents)
                .FirstOrDefaultAsync(a => a.Login == userLogin);

            ViewBag.DocumentTypes = await _context.Documents_Type
                .Where(t => t.Document_Type_Id == 1 || t.Document_Type_Id == 4)
                .ToListAsync();

            return View(user?.User_Documents ?? new List<User_Document>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProfileDocument(User_Document document)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            document.Account_Id = user.Account_Id;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Заполните все обязательные поля";
                return RedirectToAction("AddDocuments");
            }

            try
            {
                _context.User_Documents.Add(document);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Документ успешно добавлен";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToAction("AddDocuments");
        }

        private async Task<Account> GetCurrentUserAsync()
        {
            var login = User.FindFirstValue(ClaimTypes.Name);
            return await _context.Accounts
                .Include(a => a.User_Documents)
                .FirstOrDefaultAsync(a => a.Login == login)
                ?? throw new Exception("Пользователь не найден");
        }

        [HttpGet]
        public async Task<ActionResult> EditDocument(int id)
        {
            if (id == 0)
            {
                ViewBag.DocumentTypes = await _context.Documents_Type.ToListAsync();
                return View(new User_Document());
            }

            var document = await _context.User_Documents
                .Include(d => d.Document_Type)
                .FirstOrDefaultAsync(d => d.Document_Id == id);

            if (document == null) return NotFound();

            var user = await GetCurrentUserAsync();
            if (document.Account_Id != user.Account_Id) return Forbid();

            ViewBag.DocumentTypes = await _context.Documents_Type.ToListAsync();
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDocument(int id, User_Document model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            if (model.Document_Type_Id == 1) // Паспорт
            {
                if (string.IsNullOrEmpty(model.Last_Name) ||
                    string.IsNullOrEmpty(model.First_Name) ||
                    model.Document_Number == 0)
                {
                    ModelState.AddModelError("", "Заполните обязательные поля для паспорта");
                }
            }
            else if (model.Document_Type_Id == 4) // Загран
            {
                if (model.International_Document_Number == null ||
                    model.Expiration_Date == null)
                {
                    ModelState.AddModelError("", "Заполните обязательные поля для загранпаспорта");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DocumentTypes = await _context.Documents_Type.ToListAsync();
                TempData["ErrorMessage"] = "Проверьте правильность заполнения полей";
                return View(model);
            }

            try
            {
                if (id == 0)
                {
                    model.Account_Id = user.Account_Id;
                    _context.User_Documents.Add(model);
                }
                else
                {
                    var existingDocument = await _context.User_Documents
                        .FirstOrDefaultAsync(d => d.Document_Id == id);

                    if (existingDocument == null || existingDocument.Account_Id != user.Account_Id)
                        return Forbid();
                    //пиздец
                    existingDocument.Document_Type_Id = model.Document_Type_Id;
                    existingDocument.Last_Name = model.Last_Name;
                    existingDocument.First_Name = model.First_Name;
                    existingDocument.Middle_Name = model.Middle_Name;
                    existingDocument.Date_Of_Birth = model.Date_Of_Birth;
                    existingDocument.Document_Number = model.Document_Number;
                    existingDocument.International_Last_Name = model.International_Last_Name;
                    existingDocument.International_First_Name = model.International_First_Name;
                    existingDocument.International_Document_Number = model.International_Document_Number;
                    existingDocument.Expiration_Date = model.Expiration_Date;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = id == 0
                    ? "Документ успешно добавлен"
                    : "Документ обновлен";

                return RedirectToAction("ProfileDocuments");
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                TempData["ErrorMessage"] = $"Ошибка базы данных: {errorMessage}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            ViewBag.DocumentTypes = await _context.Documents_Type.ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteDocument(int id)
        {
            var document = await _context.User_Documents.FindAsync(id);
            if (document == null) return NotFound();

            var user = await GetCurrentUserAsync();
            if (document.Account_Id != user.Account_Id) return Forbid();

            try
            {
                _context.User_Documents.Remove(document);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Документ успешно удален";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToAction("ProfileDocuments");
        }

        [HttpGet]
        public async Task<IActionResult> AddDocuments()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            ViewBag.DocumentTypes = await _context.Documents_Type
                .Where(t => t.Document_Type_Id == 1 || t.Document_Type_Id == 4)
                .ToListAsync();

            return View(new User_Document { Account_Id = user.Account_Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDocuments(User_Document model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            model.Account_Id = user.Account_Id;

            // Костылим
            ModelState.Remove("Account");
            ModelState.Remove("Document_Type");

            var documentTypeExists = await _context.Documents_Type
                .AnyAsync(dt => dt.Document_Type_Id == model.Document_Type_Id);

            if (!documentTypeExists)
            {
                ModelState.AddModelError("Document_Type_Id", "Некорректный тип документа");
            }

            if (model.Document_Type_Id == 4)
            {
                if (string.IsNullOrEmpty(model.International_Last_Name))
                    ModelState.AddModelError("International_Last_Name", "Обязательное поле");

                if (string.IsNullOrEmpty(model.International_First_Name))
                    ModelState.AddModelError("International_First_Name", "Обязательное поле");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DocumentTypes = await _context.Documents_Type.ToListAsync();
                return View(model);
            }

            try
            {
                _context.User_Documents.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Документ успешно добавлен";
                return RedirectToAction("ProfileDocuments");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка: {ex.Message}");
                return View(model);
            }
        }
    }
}