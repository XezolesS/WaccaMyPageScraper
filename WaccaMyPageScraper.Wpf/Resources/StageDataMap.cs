using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.Resources;

namespace WaccaMyPageScraper.Wpf.Resources
{
    public static class StageDataMap
    {
        public static IDictionary<int, StageData> StageDatas;

        /// <summary>
        /// Initialize StageData from internal json data.<br/>
        /// This method should be called at least once.
        /// </summary>
        public static void Initialize()
        {
            StageDatas = JsonConvert.DeserializeObject<IDictionary<int, StageData>>(Properties.Resources.StageUpJson);
        }
    }

    #region StageData Data Model for Json
    public class StageData
    {
        [JsonProperty("tracks")]
        public StageTrack[] Tracks { get; set; }

        public byte[][] StageTrackImages
        {
            get
            {
                var images = new byte[3][];

                for (int i = 0; i < 3; i++)
                {
                    var musicId = this.Tracks[i].Id;
                    var filePath = Path.Combine(Directories.RecordImage, musicId + ".png");

                    if (!File.Exists(filePath))
                        return null;

                    byte[] image = null;
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        image = new byte[fs.Length];
                        while (fs.Read(image, 0, image.Length) > 0)
                        {
                            image[image.Length - 1] = (byte)fs.ReadByte();
                        }
                    }

                    images[i] = image;
                }

                return images;
            }
        }

        [JsonProperty("clearCondition")]
        public StageCondition ClearCondition { get; set; }

        [JsonProperty("lifeRestore")]
        public int LifeRestore { get; set; }

        public string LifeRestoreText => this.LifeRestore == 0 ?
            string.Empty : $"Restore {this.LifeRestore} Lifes";

        public StageData() { }

        public StageData(StageTrack[] tracks, StageCondition clearCondition, int lifeRestore)
        {
            this.Tracks = tracks;
            this.ClearCondition = clearCondition;
            this.LifeRestore = lifeRestore;
        }
    }

    public struct StageTrack
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        public string LevelText => string.Format("{0} {1}",
            this.Difficulty.ToString().ToUpperInvariant(), this.Level);

        [JsonProperty("difficulty")]
        public Difficulty Difficulty { get; set; }

        public string DifficultyColor => this.Difficulty switch
        {
            Difficulty.Normal => "#009DE6",
            Difficulty.Hard => "#FED131",
            Difficulty.Expert => "#FC06A3",
            Difficulty.Inferno => "#4A004F"
        };

        public StageTrack(int id, string title, string level, Difficulty difficulty)
        {
            this.Id = id;
            this.Title = title;
            this.Level = level;
            this.Difficulty = difficulty;
        }
    }

    public struct StageCondition
    {
        [JsonProperty("judge")]
        public string Judge { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        public StageCondition(string judge, int count)
        {
            this.Judge = judge;
            this.Count = count;
        }

        public override string ToString() => $"In {this.Count} {this.Judge}s";
    }
    #endregion
}
