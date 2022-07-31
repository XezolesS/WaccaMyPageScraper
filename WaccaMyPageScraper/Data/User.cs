using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Data
{
    public class User
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public int Rate { get; set; }

        public int StageCleared { get; set; }

        public int StageGrade { get; set; }

        public override string ToString() => string.Format("[{0}, {1}, {2}, {3}:{4}]", this.Name, this.Level, this.Rate, this.StageCleared, this.StageGrade);
    }
}
