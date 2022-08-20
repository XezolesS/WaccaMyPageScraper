using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Data
{
    public class MusicDetail : Music
    {
        public string Artist { get; set; }

        public int[] PlayCounts { get; set; }

        public int[] Scores { get; set; }

        public Rate[] Rates
        {
            get
            {
                var rates = new Rate[this.Scores.Length];
                for (int i = 0; i < rates.Length; i++)
                {
                    rates[i] = this.Scores[i] switch
                    {
                        var s when s == 1000000 => Rate.Master,
                        var s when s >= 990000 => Rate.SSS_Plus,
                        var s when s >= 980000 => Rate.SSS,
                        var s when s >= 970000 => Rate.SS_Plus,
                        var s when s >= 950000 => Rate.SS,
                        var s when s >= 930000 => Rate.S_Plus,
                        var s when s >= 900000 => Rate.S,
                        var s when s >= 850000 => Rate.AAA,
                        var s when s >= 800000 => Rate.AA,
                        var s when s >= 700000 => Rate.A,
                        var s when s >= 300000 => Rate.B,
                        var s when s >= 1 => Rate.C,
                        _ => Rate.D
                    };
                }

                return rates;
            }
        }

        public Achieve[] Achieves { get; set; }

        public override string ToString() => string.Format("[{0}] {1} - {2} ({3})\n\tPlayCounts: ({4})\n\tScores: ({5})\n\tRates: ({6})\n\tAchieves: ({7})",
            this.Id, this.Title, this.Artist, string.Join(",", this.Levels),
            string.Join(",", this.PlayCounts),
            string.Join(",", this.Scores),
            string.Join(",", this.Rates),
            string.Join(",", this.Achieves));
    }
}
