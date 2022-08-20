using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Data
{
    public class Trophy
    {
        [JsonProperty("trophyId")]
        public int Id { get; set; }

        [JsonProperty("trophyCategory")]
        public int Category { get; set; }

        [JsonProperty("trophyRarity")]
        public int Rarity { get; set; }

        [JsonProperty("trophyName")]
        public string Name { get; set; }

        [JsonProperty("trophyDescription")]
        public string Description { get; set; }

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

        public override string ToString() => string.Format("({0}) {1} - {2} | Category {3} | Rarity {4} | {5}", 
            this.Id, this.Name, this.Description, this.Category, this.Rarity, this.Obtained);
    }
}
