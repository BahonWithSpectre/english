using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace english.DbFolder
{
    public class Kurs
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Price { get; set; }

        public string BannerUrl { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime? CreatDate { get; set; }
        public string Info { get; set; }


        public string Kimge1 { get; set; }
        public string Kimge2 { get; set; }
        public string Kimge3 { get; set; }
        public string Kimge4 { get; set; }
        public string Kimge5 { get; set; }

        public string What1 { get; set; }
        public string What2 { get; set; }
        public string What3 { get; set; }

        public string AvtorName { get; set; }
        public string AvtorInfo { get; set; }
        public string AvtorImgUrl { get; set; }
    }
}
