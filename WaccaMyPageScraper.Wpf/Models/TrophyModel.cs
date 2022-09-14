using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.Models
{
    public sealed class TrophyModel : Trophy
    {
        public byte[] TrophyIcon => ImageLocator.GetTrophyIcon(this);

        public string RarityStars => new string('★', this.Rarity);

        public TrophyModel() { }

        public TrophyModel(Trophy trophy) 
            : base(trophy.Id, trophy.Category, trophy.Rarity, trophy.Name, trophy.Description, trophy.Obtained) { }

        public static TrophyModel FromTrophy(Trophy data) => new TrophyModel(data);

        public static IEnumerable<TrophyModel> FromTrophies(IEnumerable<Trophy> data)
        {
            if (data is null)
                return null;

            var trophies = new List<TrophyModel>();
            foreach (var trophy in data)
                trophies.Add(FromTrophy(trophy));

            return trophies;
        }
    }
}
