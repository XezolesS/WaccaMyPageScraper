using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Stage Stage { get; set; }

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

        public Player(string name, int level, int rate, Stage stage, int playCount, int playCountVersus, int playCountCoop, int totalRpEarned, int totalRpSpent)
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

        public override string ToString() => string.Format("{0}, Lv.{1}, Rate {2} [{3} | Played({4}, {5}(VS), {6}(CO-OP)) | RP(Earned {7}, Spent {8})]", 
            this.Name, this.Level, this.Rate,
            this.Stage,
            this.PlayCount, this.PlayCountVersus, this.PlayCountCoop,
            this.TotalRpEarned, this.TotalRpSpent);
    }
}
