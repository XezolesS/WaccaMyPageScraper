using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Data
{
    public class Music
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public Genre Genre { get; set; }

        public string[] Levels { get; set; }

        public Music()
        {
            this.Levels = new string[4];
        }

        public override string ToString() => string.Format("[{0}] {1} | {2} ({3})", 
            this.Id, this.Title, this.Genre, string.Join(",", this.Levels));
    }
}
