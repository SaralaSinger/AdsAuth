using AdsAuth.Data;
using AdsAuth.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdsAuth.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=AdsAuth; Integrated Security=true;";
        public IActionResult Index()
        {
            var repo = new Repository(_connectionString);
            var vm = new ViewModel
            {
                Ads = repo.GetAllAds()
            };
            foreach (var ad in vm.Ads)
            {
                ad.UserName = repo.GetUserName(ad.UserId);
                ad.CanDelete = ad.UserId == repo.GetUserIdByEmail(User.Identity.Name);
            }
            return View(vm);
        }
        [Authorize]
        public IActionResult MyAccount()
        {
            var repo = new Repository(_connectionString);
            var id = repo.GetUserIdByEmail(User.Identity.Name);
            var vm = new ViewModel
            {
                Ads = repo.GetAdsByUser(id)
            };
            return View(vm);
        }
        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var repo = new Repository(_connectionString);
            ad.UserId = repo.GetUserIdByEmail(User.Identity.Name);
            repo.AddAd(ad);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            var repo = new Repository(_connectionString);
            repo.DeleteAd(id);
            return RedirectToAction("Index");
        }
    }
}