using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using english.Models;
using english.DbFolder;
using Microsoft.Extensions.Logging;

namespace english.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private AppDb db;

        public HomeController(ILogger<HomeController> logger, AppDb _db)
        {
            _logger = logger;
            db = _db;
        }


        public IActionResult Index()
        {
            var kurs = db.Kurs.Take(6);

            return View(kurs);
        }


        public IActionResult LifeHack()
        {
            var list = db.Lifehacks.ToList();

            return View(list);
        }


        public IActionResult AllKurs()
        {
            var list = db.Kurs.ToList();

            return View(list);
        }

        [HttpPost]
        public IActionResult AllKurs(string cat1, string cat2, string cat3)
        {
            var list = db.Kurs.ToList();

            if (cat1 != "qw")
            {
                return Content("String !=");
            }

            return View(list);
        }




        public IActionResult Kurs(int? Id)
        {
            if (Id != null)
            {
                ViewBag.Videos = db.KursVideos.Where(x => x.KursId == Id).ToList();

                var kurs = db.Kurs.FirstOrDefault(p => p.Id == Id);
                return View(kurs);
            }
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
