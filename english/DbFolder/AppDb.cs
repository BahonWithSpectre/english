using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace english.DbFolder
{
    public class AppDb : IdentityDbContext<User>
    {
        public AppDb(DbContextOptions<AppDb> options)
            : base(options)
        {

        }

        public DbSet<Kurs> Kurs { get; set; }
        public DbSet<KursVideo> KursVideos { get; set; }
        public DbSet<UserKurs> UserKurs { get; set; }
        public DbSet<Lifehack> Lifehacks { get; set; }
    }
}
