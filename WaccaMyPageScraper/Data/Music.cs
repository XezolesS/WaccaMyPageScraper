using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Data
{
    public class Music
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string[] Levels { get; set; }

        public override string ToString() => string.Format("[{0}] {1} ({2})", 
            this.Id, this.Title, string.Join(",", this.Levels));
    }
}
