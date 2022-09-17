using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Data
{
    public class TotalRpRanking
    {
        /// <summary>
        /// A ranking of totl RP earned.
        /// </summary>
        public int Ranking { get; set; }

        public TotalRpRanking()
        {
            this.Ranking = -1;
        }

        public TotalRpRanking(int ranking)
        {
            this.Ranking = ranking;
        }

        public override string ToString() => string.Format("[{0}]", this.Ranking);
    }

    internal sealed class TotalRpRankingMap : ClassMap<TotalRpRanking>
    {
        public TotalRpRankingMap()
        {
            Map(m => m.Ranking).Index(0).Name("rp_rankings");
        }
    }
}
