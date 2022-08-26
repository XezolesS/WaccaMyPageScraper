using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Wpf.Models
{
    public class RecordModel
    {
        public string Title { get; set; }

        public string Artist { get; set; }

        public Genre Genre { get; set; }

        public Difficulty Difficulty { get; set; }

        public string DifficultyColor 
        { 
            get => this.Difficulty switch 
            { 
                Difficulty.Normal => "#009DE6",
                Difficulty.Hard => "#FED131",
                Difficulty.Expert => "#FC06A3",
                Difficulty.Inferno => "#4A004F"
            }; 
        }

        public string Level { get; set; }

        public string LevelText { get => string.Format("{0} {1}", this.Difficulty.ToString(), this.Level); }

        public int Score { get; set; }

        public int PlayCount { get; set; }

        public Rate Rate { get; set; }

        public byte[] RateImage 
        {
            get => (this.PlayCount, this.Rate) switch
            {
                var (p, r) when p > 0 && r == Rate.D => Properties.Resources.RateD,
                var (p, r) when r == Rate.C => Properties.Resources.RateC,
                var (p, r) when r == Rate.B => Properties.Resources.RateB,
                var (p, r) when r == Rate.A => Properties.Resources.RateA,
                var (p, r) when r == Rate.AA => Properties.Resources.RateAA,
                var (p, r) when r == Rate.AAA => Properties.Resources.RateAAA,
                var (p, r) when r == Rate.S => Properties.Resources.RateS,
                var (p, r) when r == Rate.S_Plus => Properties.Resources.RateS_Plus,
                var (p, r) when r == Rate.SS => Properties.Resources.RateSS,
                var (p, r) when r == Rate.SS_Plus => Properties.Resources.RateS_Plus,
                var (p, r) when r == Rate.SSS => Properties.Resources.RateSSS,
                var (p, r) when r == Rate.SSS_Plus => Properties.Resources.RateSSS_Plus,
                var (p, r) when r == Rate.Master => Properties.Resources.RateMaster,
                _ => Properties.Resources.RateNo
            };
        }

        public Achieve Achieve { get; set; }

        public RecordModel() { }

        public RecordModel(string title, string artist, Genre genre, Difficulty difficulty, string level, int score, int playCount, Rate rate, Achieve achieve)
        {
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
            return new RecordModel(data.Title,
                data.Artist,
                data.Genre,
                difficulty,
                data.Levels[(int)difficulty],
                data.Scores[(int)difficulty],
                data.PlayCounts[(int)difficulty],
                data.Rates[(int)difficulty],
                data.Achieves[(int)difficulty]);
        }
    }
}
