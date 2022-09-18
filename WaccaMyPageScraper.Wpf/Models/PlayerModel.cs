using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.Models
{
    public sealed class PlayerModel : Player
    {
        public string LevelText => "Lv. " + this.Level;

        public string RateText => "Rate " + this.Rate;

        public byte[] Icon => ImageLocator.GetPlayerIcon();

        public byte[] StageIcon => ImageLocator.GetStageIcon(this.Stage);

        public Brush RateBrush => RateColors.GetColor(this.Rate);

        public int TotalRpRanking { get; set; }

        public Brush TotalRpRankingColor => RankingColors.GetColor(this.TotalRpRanking);

        public PlayerModel() { }

        public PlayerModel(Player player, TotalRpRanking totalRpRanking)
            : base(player.Name, player.Level, player.Rate, player.Stage,
                  player.PlayCount, player.PlayCountVersus, player.PlayCountCoop,
                  player.TotalRpEarned, player.TotalRpSpent) 
        {
            this.TotalRpRanking = totalRpRanking?.Ranking ?? -1;
        }

        public static PlayerModel FromPlayer(Player data, TotalRpRanking totalRpRanking = null) 
            => new PlayerModel(data, totalRpRanking);
    }
}
