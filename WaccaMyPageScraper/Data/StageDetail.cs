using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Data
{
    public class StageDetail : Stage
    {
        public int[] Scores { get; set; }

        public int TotalScore { get => Scores.Sum(); }

        public StageDetail()
        {
            this.Scores = new int[3]; 
        }

        public override string ToString() => string.Format("Stage {0}, {1}, Score {2}({3})",
            this.Rank, this.Grade, this.TotalScore,
            string.Join(",", Scores));
    }
}
