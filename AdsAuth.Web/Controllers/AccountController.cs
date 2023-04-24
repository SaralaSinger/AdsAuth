using AdsAuth.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdsAuth.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=AdsAuth; Integrated Security=true;";

        public IActionResult Login()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new Repository(_connectionString);
            var user = repo.Login(email, password);

            if (user == null)
            {
                TempData["message"] = "Invalid Login";
                return Redirect("/account/login");
            }
            var claims = new List<Claim>
            {
                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role")))
                .Wait();

            return RedirectToAction("NewAd", "Home");
        }
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var repo = new Repository(_connectionString);
            repo.AddUser(user, password);
            return RedirectToAction("Login");
        }
    }
}
