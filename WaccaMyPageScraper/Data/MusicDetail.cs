using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Converter;
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

        public MusicDetail(string id, string title, Genre genre, string[] levels,
            string artist, int[] playCounts, int[] scores, Achieve[] achieves)
            : base(id, title, genre, levels)
        {
            this.Artist = artist;
            this.PlayCounts = playCounts;
            this.Scores = scores;
            this.Achieves = achieves;
        }

        public MusicDetail(Music music, string artist, int[] playCounts, int[] scores, Achieve[] achieves)
            : base(music.Id, music.Title, music.Genre, music.Levels)
        {
            this.Artist = artist;
            this.PlayCounts = playCounts;
            this.Scores = scores;
            this.Achieves = achieves;
        }

        public bool HasInferno() => this.Levels.Length == 4 && this.Levels[(int)Difficulty.Inferno] != "0";

        public override string ToString() => string.Format("[{0},{1},{2},{3},[{4}],[{5}],[{6}],[{7}],[{8}]]",
            this.Id, this.Title, this.Artist, (int)this.Genre, string.Join(",", this.Levels),
            string.Join(",", this.PlayCounts),
            string.Join(",", this.Scores),
            string.Join(",", this.Rates),
            string.Join(",", Array.ConvertAll(this.Achieves, s => (int)s)));
    }

    public sealed class MusicDetailMap : ClassMap<MusicDetail>
    {
        public MusicDetailMap()
        {
            Map(m => m.Id).Index(0).Name("music_id");
            Map(m => m.Title).Index(1).Name("music_title");
            Map(m => m.Artist).Index(2).Name("music_artist");
            Map(m => m.Genre).Index(3).Name("music_genre")
                .TypeConverter<EnumConverter<Genre>>();
            Map(m => m.Levels).Index(4).Name("music_levels")
                .TypeConverter<StringArrayConverter>();
            Map(m => m.PlayCounts).Index(5).Name("music_play_counts")
                .TypeConverter<Int32ArrayConverter>();
            Map(m => m.Scores).Index(6).Name("music_scores")
                .TypeConverter<Int32ArrayConverter>();
            Map(m => m.Achieves).Index(7).Name("music_achieves")
                .TypeConverter<EnumArrayConverter<Achieve>>();
        }
    }
}
