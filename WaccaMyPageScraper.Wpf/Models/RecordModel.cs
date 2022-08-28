using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.Models
{
    public class RecordModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public byte[] JacketImage
        {
            get
            {
                var filePath = Path.Combine(DataFilePath.RecordImage, this.Id + ".png");

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

                return image;
            } 
        }

        public Genre Genre { get; set; }

        public Difficulty Difficulty { get; set; }

        public string DifficultyColor  => this.Difficulty switch 
        { 
            Difficulty.Normal => "#009DE6",
            Difficulty.Hard => "#FED131",
            Difficulty.Expert => "#FC06A3",
            Difficulty.Inferno => "#4A004F"
        }; 

        public string Level { get; set; }

        public string LevelText => string.Format("{0} {1}", 
            this.Difficulty.ToString().ToUpperInvariant(), this.Level);

        public int Score { get; set; }

        public int PlayCount { get; set; }

        public Rate Rate { get; set; }

        public byte[] RateIcon  => ImageLocator.LocateRate(this.PlayCount, this.Rate);

        public Achieve Achieve { get; set; }

        public byte[] AchieveIcon => ImageLocator.LocateAchieve(this.Achieve);

        public RecordModel() { }

        public RecordModel(string id, string title, string artist, Genre genre, Difficulty difficulty, string level, int score, int playCount, Rate rate, Achieve achieve)
        {
            this.Id = id;
            this.Title = title;
            this.Artist = artist;
            this.Genre = genre;
            this.Difficulty = difficulty;
            this.Level = level;
            this.Score = score;
            this.PlayCount = playCount;
            this.Rate = rate;
            this.Achieve = achieve;
        }

        public static RecordModel FromMusicDetail(MusicDetail data, Difficulty difficulty)
        {
            return new RecordModel(
                data.Id.ToString(),
                data.Title,
                data.Artist,
                data.Genre,
                difficulty,
                data.Levels[(int)difficulty],
                data.Scores[(int)difficulty],
                data.PlayCounts[(int)difficulty],
                data.Rates[(int)difficulty],
                data.Achieves[(int)difficulty]);
        }

        public static RecordModel[] FromMusicDetail(MusicDetail data)
        {
            var toConvert = data.HasInferno() ? 4 : 3;

            var converted = new RecordModel[toConvert];
            for (int i = 0; i < toConvert; i++)
                converted[i] = FromMusicDetail(data, (Difficulty)i);

            return converted;
        }

        public static IEnumerable<RecordModel> FromMusicDetails(IEnumerable<MusicDetail> data)
        {
            var records = new List<RecordModel>();
            foreach (var musicDetail in data)
                records.AddRange(FromMusicDetail(musicDetail));

            return records;
        }

        public double LevelToNumber() => double.Parse(this.Level.Replace("+", ".1"));
    }
}
