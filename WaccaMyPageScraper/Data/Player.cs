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
    /// Structured data for containing various data of a player.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Name of a player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Level of a player.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Rate of a player.
        /// </summary>
        public int Rate { get; set; }

        /// <summary>
        /// A simple information of a stage that player has cleard.
        /// </summary>
        public StageMetadata Stage { get; set; }

        /// <summary>
        /// How many times the player has played a game.
        /// </summary>
        public int PlayCount { get; set; }

        /// <summary>
        /// How many times the player has played a versus mode.
        /// </summary>
        public int PlayCountVersus { get; set; }

        /// <summary>
        /// How many times the player has played a co-op mode.
        /// </summary>
        public int PlayCountCoop { get; set; }

        /// <summary>
        /// The total amount of RP that player has earend.
        /// </summary>
        public int TotalRpEarned { get; set; }

        /// <summary>
        /// The total amount of RP that player has spent.
        /// </summary>
        public int TotalRpSpent { get; set; }

        public Player() { }

        public Player(string name, int level, int rate, StageMetadata stage, int playCount, int playCountVersus, int playCountCoop, int totalRpEarned, int totalRpSpent)
        {
            Name = name;
            Level = level;
            Rate = rate;
            Stage = stage;
            PlayCount = playCount;
            PlayCountVersus = playCountVersus;
            PlayCountCoop = playCountCoop;
            TotalRpEarned = totalRpEarned;
            TotalRpSpent = totalRpSpent;
        }

        public override string ToString() => string.Format("[{0},{1},{2},{3},{4},{5},{6},{7},{8}]", 
            this.Name, this.Level, this.Rate,
            this.Stage,
            this.PlayCount, this.PlayCountVersus, this.PlayCountCoop,
            this.TotalRpEarned, this.TotalRpSpent);
    }

    public sealed class PlayerMap : ClassMap<Player>
    {
        public PlayerMap()
        {
            Map(m => m.Name).Index(0).Name("player_name");
            Map(m => m.Level).Index(1).Name("player_level");
            Map(m => m.Rate).Index(2).Name("player_rate");
            Map(m => m.Stage).Index(3).Name("player_stage")
                .TypeConverter<StageConverter>();
            Map(m => m.PlayCount).Index(4).Name("player_play_count");
            Map(m => m.PlayCountVersus).Index(5).Name("player_play_count_versus");
            Map(m => m.PlayCountCoop).Index(6).Name("player_play_count_coop");
            Map(m => m.TotalRpEarned).Index(7).Name("player_total_rp_earned");
            Map(m => m.TotalRpSpent).Index(8).Name("player_total_rp_spent");
        }
    }
}
