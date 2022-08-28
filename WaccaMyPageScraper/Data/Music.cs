using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
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
    /// Structured data for the musics.
    /// </summary>
    public class Music
    {
        /// <summary>
        /// Music Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Music title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Music Genre. <br/>
        /// <see cref="Genre"/>
        /// </summary>
        public Genre Genre { get; set; }

        /// <summary>
        /// Levels for each difficulties. (Max 4)
        /// </summary>
        public string[] Levels { get; set; }

        public Music()
        {
            this.Levels = new string[4];
        }

        public Music(string id, string title, Genre genre, string[] levels)
        {
            this.Id = id;
            this.Title = title;
            this.Genre = genre;
            this.Levels = levels;
        }

        public override string ToString() => string.Format("[{0},{1},{2},[{3}]]", 
            this.Id, this.Title, (int)this.Genre, string.Join(",", this.Levels));
    }

    public sealed class MusicMap : ClassMap<Music>
    {
        public MusicMap()
        {
            Map(m => m.Id).Index(0).Name("music_id");
            Map(m => m.Title).Index(1).Name("music_title");
            Map(m => m.Genre).Index(2).Name("music_genre")
                .TypeConverter<EnumConverter<Genre>>();
            Map(m => m.Levels).Index(3).Name("music_levels")
                .TypeConverter<StringArrayConverter>();
        }
    }
}
