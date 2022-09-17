using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Data
{
    public class TotalScoreRankings
    {
        /// <summary>
        /// Total score ranking on Normal.
        /// </summary>
        public int Normal { get; set; }

        /// <summary>
        /// Total score ranking on Hard.
        /// </summary>
        public int Hard { get; set; }

        /// <summary>
        /// Total score ranking on Expert.
        /// </summary>
        public int Expert { get; set; }

        /// <summary>
        /// Total score ranking on Inferno.
        /// </summary>
        public int Inferno { get; set; }

        public int GetRanking(Difficulty difficulty) => difficulty switch
        {
            Difficulty.Normal => this.Normal,
            Difficulty.Hard => this.Hard,
            Difficulty.Expert => this.Expert,
            Difficulty.Inferno => this.Inferno,
        };

        public TotalScoreRankings()
        {
            this.Normal = -1;
            this.Hard = -1;
            this.Expert = -1;
            this.Inferno = -1;
        }

        public TotalScoreRankings(int normal, int hard, int expert, int inferno)
        {
            this.Normal = normal;
            this.Hard = hard;
            this.Expert = expert;
            this.Inferno = inferno;
        }

        public TotalScoreRankings(int[] rankings)
        {
            this.Normal = rankings[0];
            this.Hard = rankings[1];
            this.Expert = rankings[2];
            this.Inferno = rankings[3];
        }

        public override string ToString() => string.Format("[{0},{1},{2},{3}]",
            this.Normal, this.Hard, this.Expert, this.Inferno);
    }

    internal sealed class TotalScoreRankingsMap : ClassMap<TotalScoreRankings>
    {
        public TotalScoreRankingsMap()
        {
            Map(m => m.Normal).Index(0).Name("normal_rankings");
            Map(m => m.Hard).Index(1).Name("hard_rankings");
            Map(m => m.Expert).Index(2).Name("expert_rankings");
            Map(m => m.Inferno).Index(3).Name("inferno_rankings");
        }
    }
}
