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
        /// The highest stage that player has passed.
        /// </summary>
        public int StageCleared { get; set; }

        /// <summary>
        /// The grade of a stage that player has passed.
        /// </summary>
        public StageGrade StageGrade { get; set; }

        public override string ToString() => string.Format("[{0}, {1}, {2}, ({3},{4},{5}), {6}:{7}]", 
            this.Name, this.Level, this.Rate, 
            this.PlayCount, this.PlayCountVersus, this.PlayCountCoop,
            this.StageCleared, this.StageGrade);
    }
}
