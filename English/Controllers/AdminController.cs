using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using english.DbFolder;
using english.ViewModel.AdminViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace english.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public readonly AppDb db;
        public IHostingEnvironment env;


        public AdminController(AppDb _db, IHostingEnvironment _env)
        {
            db = _db;
            env = _env;
        }

        //---User Actions
        public async Task<IActionResult> UserList(int? page = 1)
        {
            ViewBag.Count = db.Users.Where(x => x.UserName != "Admin").Count();
            ViewBag.Page = page;

            var pager = new Pager(ViewBag.Count, page);

            UsersView uv = new UsersView
            {
                Users = await db.Users.Where(x => x.UserName != "Admin").Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToListAsync(),
                Pager = pager
            };

            return View(uv);
        }

        public async Task<IActionResult> AboutUser(string id)
        {
            AboutUserView auv = new AboutUserView()
            {
                User = db.Users.FirstOrDefault(x => x.Id == id),
                Kurs = await db.Kurs.ToListAsync(),
                UserKurs = await db.UserKurs.Where(x => x.UserId == id).ToListAsync()
            };

            return View(auv);
        }
        [HttpPost]
        public async Task<IActionResult> AboutUser(string userId, int kursId)
        {
            if (userId != null && kursId != 0)
            {
                var have = await db.UserKurs.FirstOrDefaultAsync(x => x.UserId == userId && x.KursId == kursId);
                if (have != null)
                {
                    db.Remove(have);
                    await db.SaveChangesAsync();
                }
                else
                {
                    UserKurs uk = new UserKurs { UserId = userId, KursId = kursId };

                    await db.AddAsync(uk);
                    await db.SaveChangesAsync();
                }
                AboutUserView auv = new AboutUserView()
                {
                    User = db.Users.FirstOrDefault(x => x.Id == userId),
                    Kurs = await db.Kurs.ToListAsync(),
                    UserKurs = await db.UserKurs.Where(x => x.UserId == userId).ToListAsync()
                };
                return View(auv);
            }

            return RedirectToAction("UserList");
        }



        //---LifeHack Actions

        public IActionResult LifehackList()
        {
            var list = db.Lifehacks.ToList();
            return View(list);
        }

        public IActionResult AddLifehack()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddLifehack(Lifehack model, IFormFile PhotoUrl)
        {
            if (model != null)
            {
                if (PhotoUrl == null || PhotoUrl.Length == 0)
                {
                    model.PhotoUrl = "default.jpg";
                }
                else
                {
                    var imgname = DateTime.Now.ToString("MMddHHmmss") + PhotoUrl.FileName;
                    string path_Root = env.WebRootPath;

                    string path_to_Images = path_Root + "/lifehack/" + imgname;
                    using (var stream = new FileStream(path_to_Images, FileMode.Create))
                    {
                        await PhotoUrl.CopyToAsync(stream);
                    }
                    model.PhotoUrl = imgname;
                }
                await db.Lifehacks.AddAsync(model);
                await db.SaveChangesAsync();

                return RedirectToAction("LifehackList");
            }
            else
            {
                return RedirectToAction("LifehackList", "Admin");
            }
        }


        public IActionResult EditLifehack(int Id)
        {
            var lh = db.Lifehacks.FirstOrDefault(p => p.Id == Id);
            return View(lh);
        }
        [HttpPost]
        public async Task<IActionResult> EditLifehack(Lifehack model, IFormFile PhotoUrl)
        {
            if (PhotoUrl != null)
            {
                var imgname = DateTime.Now.ToString("MMddHHmmss") + PhotoUrl.FileName;
                string path_Root = env.WebRootPath;

                string path_to_Images = path_Root + "/lifehack/" + imgname;
                using (var stream = new FileStream(path_to_Images, FileMode.Create))
                {
                    await PhotoUrl.CopyToAsync(stream);
                }
                model.PhotoUrl = imgname;
                db.Update(model);
            }
            else
            {
                var lh = await db.Lifehacks.FirstOrDefaultAsync(x => x.Id == model.Id);
                lh.PhotoUrl = lh.PhotoUrl;
                lh.Title = model.Title;
                lh.VideoUrl = model.VideoUrl;
            }
            await db.SaveChangesAsync();

            return RedirectToAction("LifehackList");
        }


        public IActionResult DeleteLifehack(int Id)
        {
            var lh = db.Lifehacks.FirstOrDefault(p => p.Id == Id);

            db.Lifehacks.Remove(lh);
            db.SaveChanges();

            return RedirectToAction("LifehackList");
        }




        //---Kurs Actions

        public IActionResult KursList()
        {
            var kurs = db.Kurs.ToList();
            return View(kurs);
        }

        public IActionResult KursCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> KursCreate(Kurs kurs, IFormFileCollection files)
        {
            foreach (var file in files)
            {
                if (file == null) return Content("Файл не найден!");

                var img1 = DateTime.Now.ToString("MMddHHmmss") + file.FileName;
                using (var stream = new FileStream(env.WebRootPath + "/kurs/" + img1, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                };
            }
            kurs.CreatDate = DateTime.Now.AddDays(10);
            kurs.BannerUrl = DateTime.Now.ToString("MMddHHmmss") + files[0].FileName;
            kurs.PhotoUrl = DateTime.Now.ToString("MMddHHmmss") + files[1].FileName;
            kurs.AvtorImgUrl = DateTime.Now.ToString("MMddHHmmss") + files[2].FileName;

            db.Kurs.Add(kurs);
            db.SaveChanges();

            return RedirectToAction("KursList", "Admin");
        }

        public IActionResult KursEdit(int Id)
        {
            var kurs = db.Kurs.FirstOrDefault(p => p.Id == Id);
            ViewBag.KursVideos = db.KursVideos.Where(x => x.KursId == Id);

            return View(kurs);
        }


        [HttpPost]
        public async Task<IActionResult> KursEdit(Kurs kurs, IFormFile banner, IFormFile fon, IFormFile avtor)
        {
            if (banner != null)
            {
                var imgname = DateTime.Now.ToString("MMddHHmmss") + banner.FileName;
                using (var stream = new FileStream(env.WebRootPath + "/kurs/" + imgname, FileMode.Create))
                {
                    await banner.CopyToAsync(stream);
                };
                kurs.BannerUrl = imgname;
            }
            //photo
            if (fon != null)
            {
                var imgname = DateTime.Now.ToString("MMddHHmmss") + fon.FileName;
                using (var stream = new FileStream(env.WebRootPath + "/kurs/" + imgname, FileMode.Create))
                {
                    await fon.CopyToAsync(stream);
                };
                kurs.PhotoUrl = imgname;
            }
            //avatar
            if (avtor != null)
            {
                var imgname = DateTime.Now.ToString("MMddHHmmss") + avtor.FileName;
                using (var stream = new FileStream(env.WebRootPath + "/kurs/" + imgname, FileMode.Create))
                {
                    await avtor.CopyToAsync(stream);
                };
                kurs.AvtorImgUrl = imgname;
            }

            db.Kurs.Update(kurs);
            await db.SaveChangesAsync();

            return RedirectToAction("KursEdit", new { kurs.Id });
        }


        public IActionResult DeleteKurs(int? Id)
        {
            if (Id != null)
            {
                var kurs = db.Kurs.FirstOrDefault(x => x.Id == Id);

                db.Kurs.Remove(kurs);
                db.SaveChanges();
            }

            return RedirectToAction("KursList", "Admin");
        }




        public IActionResult AddVideo(int? Id)
        {
            ViewBag.Kurs = db.Kurs.FirstOrDefault(x => x.Id == Id);
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddVideo(int kursId, string VideoName, string Info, string VideoUrl, IFormFile PhotoUrl, bool Free, string WorkUrl)
        {

            if (kursId != 0)
            {
                KursVideo kv = new KursVideo { KursId = kursId, VideoName = VideoName, Info = Info, VideoUrl = VideoUrl, WorkUrl = WorkUrl };

                if (Free == true)
                {
                    kv.Free = true;
                }
                if (PhotoUrl == null || PhotoUrl.Length == 0)
                {
                    kv.PhotoUrl = "default.jpg";
                }
                else
                {
                    var imgname = DateTime.Now.ToString("MMddHHmmss") + PhotoUrl.FileName;
                    string path_Root = env.WebRootPath;

                    string path_to_Images = path_Root + "/kursvideo/" + imgname;
                    using (var stream = new FileStream(path_to_Images, FileMode.Create))
                    {
                        await PhotoUrl.CopyToAsync(stream);
                    }

                    kv.PhotoUrl = imgname;
                }
                db.KursVideos.Add(kv);
                await db.SaveChangesAsync();

                return RedirectToAction("KursEdit", new { Id = kv.KursId });
            }
            else
            {
                return RedirectToAction("KursList", "Admin");
            }

        }


        public IActionResult EditVideo(int Id)
        {
            var video = db.KursVideos.FirstOrDefault(p => p.Id == Id);

            return View(video);
        }


        [HttpPost]
        public async Task<IActionResult> EditVideo(int? kursId, int? videoId, KursVideo video, IFormFile PhotoUrl, bool Free, string WorkUrl)
        {
            if (kursId != null)
            {
                var thisVideo = db.KursVideos.FirstOrDefault(x => x.Id == videoId);
                if (PhotoUrl == null || PhotoUrl.Length == 0)
                {
                    video.PhotoUrl = thisVideo.PhotoUrl;
                }
                else
                {
                    var imgname = DateTime.Now.ToString("MMddHHmmss") + PhotoUrl.FileName;
                    string path_Root = env.WebRootPath;

                    string path_to_Images = path_Root + "/kursvideo/" + imgname;
                    using (var stream = new FileStream(path_to_Images, FileMode.Create))
                    {
                        await PhotoUrl.CopyToAsync(stream);
                    }

                    video.PhotoUrl = imgname;
                }

                thisVideo.VideoName = video.VideoName;
                thisVideo.Info = video.Info;
                thisVideo.VideoUrl = video.VideoUrl;
                thisVideo.PhotoUrl = video.PhotoUrl;
                thisVideo.KursId = (int)kursId;
                thisVideo.WorkUrl = video.WorkUrl;

                if(Free == true)
                {
                    thisVideo.Free = true;
                }

                await db.SaveChangesAsync();

                return RedirectToAction("KursEdit", new { Id = video.KursId });
            }
            else
            {
                return RedirectToAction("KursList", "Admin");
            }

        }


        public IActionResult DeleteVideo(int Id)
        {
            var video = db.KursVideos.FirstOrDefault(p => p.Id == Id);

            db.KursVideos.Remove(video);
            db.SaveChanges();

            return RedirectToAction("KursEdit", new { Id = video.KursId });
        }

















        //public IActionResult FreeVideoList()
        //{
        //    return View(db.Categories.ToList());
        //}



        //public IActionResult FreeVideoAdd()
        //{
        //    return View();
        //}


        //[HttpPost]
        //public async Task<IActionResult> FreeVideoAdd(Category model)
        //{
        //    Category c = new Category { CategoryName = model.CategoryName };

        //    db.Categories.Add(c);

        //    await db.SaveChangesAsync();

        //    return RedirectToAction("FreeVideoList");


        //}
    }
}