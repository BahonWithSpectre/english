using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using english.Models;
using english.DbFolder;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace english.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private AppDb db;
        UserManager<User> _userManager;
        SignInManager<User> _signInManager;

        public HomeController(ILogger<HomeController> logger, AppDb _db, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            db = _db;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public IActionResult Index()
        {
            ViewBag.LifeHack = db.Lifehacks.Take(3);
            var kurs = db.Kurs.Take(6);
            return View(kurs);
        }


        public async Task<IActionResult> LifeHack()
        {
            var list = await db.Lifehacks.ToListAsync();

            return View(list);
        }


        public IActionResult LifeView(int Id)
        {
            var list = db.Lifehacks.ToList();

            var one = db.Lifehacks.Where(p => p.Id == Id).FirstOrDefault();

            LifehackView lhv = new LifehackView { LifeHacks = list, Lifehack =  one};

            return View(lhv);
        }


        public async Task<IActionResult> AllKurs()
        {
            var list = await db.Kurs.ToListAsync();

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




        public async Task<IActionResult> Kurs(int? Id)
        {
            if (Id != null)
            {
                ViewBag.Videos = db.KursVideos.Where(x => x.KursId == Id).ToList();

                var kurs = db.Kurs.FirstOrDefault(p => p.Id == Id);

                if(User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);

                    var stat = await db.UserKurs.Where(p => p.UserId == user.Id && p.KursId == Id).FirstOrDefaultAsync();

                    if(stat != null)
                    {
                        ViewBag.Stats = 1;
                    }
                }
                

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
