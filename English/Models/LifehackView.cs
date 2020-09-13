using english.DbFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace english.Models
{
    public class LifehackView
    {
        public IEnumerable<Lifehack> LifeHacks { get; set; }

        public Lifehack Lifehack { get; set; }
    }
}
