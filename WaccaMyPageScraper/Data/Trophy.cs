using CsvHelper.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Data
{
    /// <summary>
    /// Structured data for the trophies.
    /// </summary>
    public class Trophy
    {
        /// <summary>
        /// Trophy Id.
        /// </summary>
        [JsonProperty("trophyId")]
        public int Id { get; set; }

        /// <summary>
        /// Trophy category.
        /// </summary>
        [JsonProperty("trophyCategory")]
        public int Category { get; set; }

        /// <summary>
        /// Trophy rarity. (1 to 4)
        /// </summary>
        [JsonProperty("trophyRarity")]
        public int Rarity { get; set; }

        /// <summary>
        /// Trophy name.
        /// </summary>
        [JsonProperty("trophyName")]
        public string Name { get; set; }

        /// <summary>
        /// Trophy description.
        /// </summary>
        [JsonProperty("trophyDescription")]
        public string Description { get; set; }

        /// <summary>
        /// True if the player has obtained it, false if not.
        /// </summary>
        [JsonProperty("isHavingTrophy")]
        public bool Obtained { get; set; }

        public Trophy() { }

        public Trophy(int id, int category, int rarity, string name, string description, bool obtained)
        {
            this.Id = id;
            this.Category = category;
            this.Rarity = rarity;
            this.Name = name;
            this.Description = description;
            this.Obtained = obtained;
        }

        public override string ToString() => string.Format("[{0},{1},{2},{3},{4},{5}]", 
            this.Id, this.Name, this.Description, this.Category, this.Rarity, this.Obtained);
    }

    public sealed class TrophyMap : ClassMap<Trophy>
    {
        public TrophyMap()
        {
            Map(m => m.Id).Index(0).Name("trophy_id");
            Map(m => m.Name).Index(1).Name("trophy_name");
            Map(m => m.Description).Index(2).Name("trophy_description");
            Map(m => m.Category).Index(3).Name("trophy_category");
            Map(m => m.Rarity).Index(4).Name("trophy_rarity");
            Map(m => m.Obtained).Index(5).Name("trophy_obtained");
        }
    }
}
