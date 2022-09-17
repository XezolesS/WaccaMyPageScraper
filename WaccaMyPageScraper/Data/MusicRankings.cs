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
    public class MusicRankings : MusicMetadata
    {
        /// <summary>
        /// Global rankings of each difficulties. (Max 4)
        /// </summary>
        public int[] Rankings { get; set; }

        public int GetRanking(Difficulty difficulty) => this.Rankings[(int)difficulty];

        public MusicRankings() : base()
        {
            this.Rankings = new int[4];
        }

        public MusicRankings(string id, string title, Genre genre, string[] levels, int[] rankings)
            : base(id, title, genre, levels)
        {
            this.Rankings = rankings;
        }

        public MusicRankings(MusicMetadata meta, int[] rankings)
            : base(meta.Id, meta.Title, meta.Genre, meta.Levels)
        {
            this.Rankings = rankings;
        }

        public override string ToString() => string.Format("[{0},{1},{2},[{3}],[{4}]]",
            this.Id, this.Title, (int)this.Genre, string.Join(",", this.Levels),
            string.Join(",", this.Rankings));
    }

    internal sealed class MusicRankingsMap : ClassMap<MusicRankings>
    {
        public MusicRankingsMap()
        {
            Map(m => m.Id).Index(0).Name("music_id");
            Map(m => m.Title).Index(1).Name("music_title");
            Map(m => m.Genre).Index(2).Name("music_genre")
                .TypeConverter<EnumConverter<Genre>>();
            Map(m => m.Levels).Index(3).Name("music_levels")
                .TypeConverter<StringArrayConverter>();
            Map(m => m.Rankings).Index(4).Name("music_rankings")
                .TypeConverter<Int32ArrayConverter>();
        }
    }
}
