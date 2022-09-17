using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.Models
{
    public sealed class RecordModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public byte[] JacketImage
        {
            get
            {
                var filePath = Path.Combine(Directories.RecordImage, this.Id + ".png");

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

        public string GenreText => this.Genre switch
        {
            Genre.AnimePop => WaccaMyPageScraper.Localization.Data.Genre_AnimePop,
            Genre.Vocaloid => WaccaMyPageScraper.Localization.Data.Genre_Vocaloid,
            Genre.Touhou => WaccaMyPageScraper.Localization.Data.Genre_Touhou,
            Genre.TwoPointFive => WaccaMyPageScraper.Localization.Data.Genre_TwoPointFive,
            Genre.Variety => WaccaMyPageScraper.Localization.Data.Genre_Variety,
            Genre.Original => WaccaMyPageScraper.Localization.Data.Genre_Original,
            Genre.TanoC => WaccaMyPageScraper.Localization.Data.Genre_TanoC,
        };

        public Brush GenreColor => this.Genre switch
        {
            Genre.AnimePop => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A8FFF")),
            Genre.Vocaloid => new LinearGradientBrush(
                    (Color)ColorConverter.ConvertFromString("#0CB092"), 
                    (Color)ColorConverter.ConvertFromString("#87FFE9"),
                    new Point(0, 0), 
                    new Point(1, 1)),
            Genre.Touhou => new LinearGradientBrush(
                    (Color)ColorConverter.ConvertFromString("#E60000"),
                    (Color)ColorConverter.ConvertFromString("#171212"),
                    new Point(0, 0),
                    new Point(1, 1)),
            Genre.TwoPointFive => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF970F")),
            Genre.Variety => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6EFF81")),
            Genre.Original => new LinearGradientBrush(
                    (Color)ColorConverter.ConvertFromString("#FF1C9D"),
                    (Color)ColorConverter.ConvertFromString("#1CBFFF"),
                    new Point(0, 0),
                    new Point(1, 1)),
            Genre.TanoC => new LinearGradientBrush(
                    (Color)ColorConverter.ConvertFromString("#787878"),
                    (Color)ColorConverter.ConvertFromString("#2E2E2E"),
                    new Point(0, 0),
                    new Point(1, 1))
        };

        public Difficulty Difficulty { get; set; }

        public string DifficultyColor => this.Difficulty switch 
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

        public byte[] RateIcon  => ImageLocator.GetRateIcon(this.PlayCount, this.Rate);

        public Achieve Achieve { get; set; }

        public byte[] AchieveIcon => ImageLocator.GetAchieveIcon(this.Achieve);

        public int Ranking { get; set; }

        public Brush RankingColor => this.Ranking switch
        {
            var r when r == -1 => new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
            var r when r == 1 => new LinearGradientBrush(
                    new GradientStopCollection()
                    {
                        new GradientStop(Color.FromRgb(200, 50, 255), 0.0),
                        new GradientStop(Color.FromRgb(255, 50, 140), 0.3),
                        new GradientStop(Color.FromRgb(255, 50, 140), 0.6),
                        new GradientStop(Color.FromRgb(255, 200, 50), 1.0),
                    }, 45.0),
            var r when r == 2 => new LinearGradientBrush(
                    new GradientStopCollection()
                    {
                        new GradientStop(Color.FromRgb(255, 200, 50), 0.0),
                        new GradientStop(Color.FromRgb(200, 160, 30), 1.0),
                    }, 45.0),
            var r when r == 3 => new LinearGradientBrush(
                    new GradientStopCollection()
                    {
                        new GradientStop(Color.FromRgb(50, 150, 255), 0.0),
                        new GradientStop(Color.FromRgb(30, 120, 200), 1.0),
                    }, 45.0),
            var r when r <= 100 => new SolidColorBrush(Color.FromRgb(100, 200, 30)),
            _ => new SolidColorBrush(Color.FromRgb(200, 200, 200)),
        };

        public RecordModel() { }

        public RecordModel(string id, string title, string artist, Genre genre, Difficulty difficulty, string level, int score, int playCount, Rate rate, Achieve achieve, int ranking)
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
            this.Ranking = ranking;
        }

        public static RecordModel FromMusic(Music data, MusicRankings rankings, Difficulty difficulty) => 
            new RecordModel(
                data.Id.ToString(),
                data.Title,
                data.Artist,
                data.Genre,
                difficulty,
                data.Levels[(int)difficulty],
                data.Scores[(int)difficulty],
                data.PlayCounts[(int)difficulty],
                data.Rates[(int)difficulty],
                data.Achieves[(int)difficulty],
                rankings?.GetRanking(difficulty) ?? -1);

        public static RecordModel[] FromMusic(Music data, MusicRankings rankings)
        {
            var toConvert = data.HasInferno() ? 4 : 3;

            var converted = new RecordModel[toConvert];
            for (int i = 0; i < toConvert; i++)
                converted[i] = FromMusic(data, rankings, (Difficulty)i);

            return converted;
        }

        public static IEnumerable<RecordModel> FromMusics(IEnumerable<Music> data, IEnumerable<MusicRankings> rankings)
        {
            if (data is null)
                return null;

            if (data.Count() == 0)
                return null;

            var records = new List<RecordModel>();
            foreach (var music in data)
                records.AddRange(FromMusic(music, 
                    rankings?.Count() == 0 ? null : rankings?.First(r => r.Id == music.Id)));

            return records; 
        }

        public double LevelToNumber() => double.Parse(this.Level.Replace("+", ".1"));
    }
}
