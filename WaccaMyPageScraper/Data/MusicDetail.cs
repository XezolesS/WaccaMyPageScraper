using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Data
{
    /// <summary>
    /// Structured data for the detailed music data.
    /// </summary>
    public class MusicDetail : Music
    {
        /// <summary>
        /// Artist of the music.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// Play counts for each difficulties. (Max 4)
        /// </summary>
        public int[] PlayCounts { get; set; }

        /// <summary>
        /// Scores for each difficulties. (Max 4)
        /// </summary>
        public int[] Scores { get; set; }

        /// <summary>
        /// Rates for each difficulties. (Max 4) <br/>
        /// Automatically calculate from scores.
        /// </summary>
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

        /// <summary>
        /// Achieved rate for each difficulties. (Max 4) <br/>
        /// <see cref="Achieve"/>
        /// </summary>
        public Achieve[] Achieves { get; set; }

        public MusicDetail() : base()
        {
            this.PlayCounts = new int[4];
            this.Scores = new int[4];
            this.Achieves = new Achieve[4];
        }

        public MusicDetail(int id, string title, Genre genre, string[] levels, 
            string artist, int[] playCounts, int[] scores, Achieve[] achieves) 
            : base(id, title, genre, levels)
        {
            Artist = artist;
            PlayCounts = playCounts;
            Scores = scores;
            Achieves = achieves;
        }

        public override string ToString() => string.Format("[{0}] {1} - {2} | {3} ({4}) | PlayCounts: ({5}) | Scores: ({6}) | Rates: ({7}) | Achieves: ({8})",
            this.Id, this.Title, this.Artist, this.Genre, string.Join(",", this.Levels),
            string.Join(",", this.PlayCounts),
            string.Join(",", this.Scores),
            string.Join(",", this.Rates),
            string.Join(",", this.Achieves));
    }
}
