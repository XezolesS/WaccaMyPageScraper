using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.Localization;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.Models
{
    public sealed class RecordDetailModel
    {
        /// <summary>
        /// Music counts of the difficulty.
        /// </summary>
        public Dictionary<Difficulty, int> MusicCounts { get; set; }

        /// <summary>
        /// Cleared music counts of the difficulty.
        /// </summary>
        public Dictionary<Difficulty, int> ClearedCounts => new Dictionary<Difficulty, int>
        {
            { Difficulty.Normal, GetCleared(Difficulty.Normal) },
            { Difficulty.Hard, GetCleared(Difficulty.Hard) },
            { Difficulty.Expert, GetCleared(Difficulty.Expert) },
            { Difficulty.Inferno, GetCleared(Difficulty.Inferno) },
        };

        /// <summary>
        /// Play counts of the difficulty.
        /// </summary>
        public Dictionary<Difficulty, int> PlayCounts { get; set; }

        /// <summary>
        /// Total scores of the difficulty.
        /// </summary>
        public Dictionary<Difficulty, int> TotalScores { get; set; }

        public Dictionary<Difficulty, int> MaxTotalScores => new Dictionary<Difficulty, int>
        {
            { Difficulty.Normal, MusicCounts[Difficulty.Normal] * 1000000 },
            { Difficulty.Hard, MusicCounts[Difficulty.Hard] * 1000000 },
            { Difficulty.Expert, MusicCounts[Difficulty.Expert] * 1000000 },
            { Difficulty.Inferno, MusicCounts[Difficulty.Inferno] * 1000000 },
        };

        /// <summary>
        /// Rates of the difficulty.
        /// </summary>
        public Dictionary<Tuple<Difficulty, Rate>, int> Rates { get; set; }

        public Dictionary<Difficulty, ISeries[]> RateSeries => new Dictionary<Difficulty, ISeries[]>
        {
            { Difficulty.Normal, BuildRateSeries(Difficulty.Normal) },
            { Difficulty.Hard, BuildRateSeries(Difficulty.Hard) },
            { Difficulty.Expert, BuildRateSeries(Difficulty.Expert) },
            { Difficulty.Inferno, BuildRateSeries(Difficulty.Inferno) },
        };

        public Axis[] RateXAxes => new Axis[]
        {
            new Axis {
                Labels = new string[] { WaccaMyPageScraper.Localization.Data.Rate },
            },
        };

        public Dictionary<Difficulty, Axis[]> RateYAxes => new Dictionary<Difficulty, Axis[]>
        {
            { Difficulty.Normal, new Axis[] { new Axis { MinLimit = 0, MaxLimit = MusicCounts[Difficulty.Normal],} } },
            { Difficulty.Hard, new Axis[] { new Axis { MinLimit = 0, MaxLimit = MusicCounts[Difficulty.Hard],} } },
            { Difficulty.Expert, new Axis[] { new Axis { MinLimit = 0, MaxLimit = MusicCounts[Difficulty.Expert],} } },
            { Difficulty.Inferno, new Axis[] { new Axis { MinLimit = 0, MaxLimit = MusicCounts[Difficulty.Inferno],} } },
        };

        /// <summary>
        /// Achieves of the difficulty.
        /// </summary>
        public Dictionary<Tuple<Difficulty, Achieve>, int> Achieves { get; set; }

        public Dictionary<Difficulty, ISeries[]> AchieveSeries => new Dictionary<Difficulty, ISeries[]>
        {
            { Difficulty.Normal, BuildAchieveSeries(Difficulty.Normal) },
            { Difficulty.Hard, BuildAchieveSeries(Difficulty.Hard) },
            { Difficulty.Expert, BuildAchieveSeries(Difficulty.Expert) },
            { Difficulty.Inferno, BuildAchieveSeries(Difficulty.Inferno) },
        };

        /// <summary>
        /// Total score rankings.
        /// </summary>
        public TotalScoreRankings TotalScoreRankings { get; set; }

        public Dictionary<Difficulty, Brush> TotalScoreRankingColors => new Dictionary<Difficulty, Brush>
        {
            { Difficulty.Normal, RankingColors.GetColor(this.TotalScoreRankings.Normal) },
            { Difficulty.Hard, RankingColors.GetColor(this.TotalScoreRankings.Hard) },
            { Difficulty.Expert, RankingColors.GetColor(this.TotalScoreRankings.Expert) },
            { Difficulty.Inferno, RankingColors.GetColor(this.TotalScoreRankings.Inferno) },
        };

        public RecordDetailModel() { }

        public RecordDetailModel(RecordModel[] records, TotalScoreRankings totalScoreRankings) => ConvertFrom(records, totalScoreRankings);

        public static RecordDetailModel ConvertFrom(RecordModel[] records, TotalScoreRankings totalScoreRankings)
        {
            if (records is null || records.Length == 0)
                return null;

            var result = new RecordDetailModel();
            result.TotalScoreRankings = totalScoreRankings;

            var musicCounts = new Dictionary<Difficulty, int>();
            var playCounts = new Dictionary<Difficulty, int>();
            var totalScores = new Dictionary<Difficulty, int>();
            var rates = new Dictionary<Tuple<Difficulty, Rate>, int>();
            var achieves = new Dictionary<Tuple<Difficulty, Achieve>, int>();

            // Dictionaries initializing
            for (Difficulty difficulty = Difficulty.Normal; difficulty <= Difficulty.Inferno; difficulty++)
            {
                musicCounts.Add(difficulty, 0);
                playCounts.Add(difficulty, 0);
                totalScores.Add(difficulty, 0);

                for (Rate rate = Rate.NoRate; rate <= Rate.Master; rate++)
                {
                    var rateTuple = GetRateTuple(difficulty, rate);
                    rates.Add(rateTuple, 0);
                }

                for (Achieve achieve = Achieve.NoAchieve; achieve <= Achieve.AllMarvelous; achieve++)
                {
                    var achieveTuple = GetAchieveTuple(difficulty, achieve);
                    achieves.Add(achieveTuple, 0);
                }
            }

            foreach (var record in records)
            {
                musicCounts[record.Difficulty]++;
                playCounts[record.Difficulty] += record.PlayCount;
                totalScores[record.Difficulty] += record.Score;

                var rateTuple = GetRateTuple(record.Difficulty, record.Rate);
                rates[rateTuple]++;

                var achieveTuple = GetAchieveTuple(record.Difficulty, record.Achieve);
                achieves[achieveTuple]++;
            }

            result.MusicCounts = musicCounts;
            result.PlayCounts = playCounts;
            result.TotalScores = totalScores;
            result.Rates = rates;
            result.Achieves = achieves;

            return result;
        }

        private static Tuple<Difficulty, Rate> GetRateTuple(Difficulty difficulty, Rate rate)
            => new Tuple<Difficulty, Rate>(difficulty, rate);

        private static Tuple<Difficulty, Achieve> GetAchieveTuple(Difficulty difficulty, Achieve achieve)
            => new Tuple<Difficulty, Achieve>(difficulty, achieve);

        private ISeries[] BuildRateSeries(Difficulty difficulty)
        {
            var result = new ISeries[(int)Rate.Master];

            for (Rate rate = Rate.D; rate <= Rate.Master; rate++)
            {
                var rateTuple = GetRateTuple(difficulty, rate);
                result[(int)rate - 1] = new ColumnSeries<int>
                {
                    Name = WaccaMyPageScraper.Localization.RateTextParser.Parse(rate),
                    Values = new int[] { this.Rates[rateTuple] },
                    Fill = RatePaints.GetPaint(rate),
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsPaint = RatePaints.GetPaint(rate),
                    DataLabelsSize = 10,
                };
            };

            return result;
        }

        private ISeries[] BuildAchieveSeries(Difficulty difficulty) => new ISeries[]
        {
            new PieSeries<int>
            {
                Name = WaccaMyPageScraper.Localization.Data.Achieve_NoAchieve,
                Values = new List<int> { this.Achieves[GetAchieveTuple(difficulty, Achieve.NoAchieve)] },
                RelativeInnerRadius = 75,
                HoverPushout = 0,
                Fill = AchievePaints.NoAchieve,
            },
            new PieSeries<int>
            {
                Name = WaccaMyPageScraper.Localization.Data.Achieve_Clear,
                Values = new List<int> { this.Achieves[GetAchieveTuple(difficulty, Achieve.Clear)] },
                RelativeInnerRadius = 75,
                HoverPushout = 0,
                Fill = AchievePaints.Clear,
            },
            new PieSeries<int>
            {
                Name = WaccaMyPageScraper.Localization.Data.Achieve_Missless,
                Values = new List<int> { this.Achieves[GetAchieveTuple(difficulty, Achieve.Missless)] },
                RelativeInnerRadius = 75,
                HoverPushout = 0,
                Fill = AchievePaints.Missless,
            },
            new PieSeries<int>
            {
                Name = WaccaMyPageScraper.Localization.Data.Achieve_FullCombo,
                Values = new List<int> { this.Achieves[GetAchieveTuple(difficulty, Achieve.FullCombo)] },
                RelativeInnerRadius = 75,
                HoverPushout = 0,
                Fill = AchievePaints.FullCombo,
            },
            new PieSeries<int>
            {
                Name = WaccaMyPageScraper.Localization.Data.Achieve_AllMarvelous,
                Values = new List<int> { this.Achieves[GetAchieveTuple(difficulty, Achieve.AllMarvelous)] },
                RelativeInnerRadius = 75,
                HoverPushout = 0,
                Fill = AchievePaints.AllMarvelous,
            },
        };

        private int GetCleared(Difficulty difficulty) =>
            this.Achieves[GetAchieveTuple(difficulty, Achieve.Clear)]
            + this.Achieves[GetAchieveTuple(difficulty, Achieve.Missless)]
            + this.Achieves[GetAchieveTuple(difficulty, Achieve.FullCombo)]
            + this.Achieves[GetAchieveTuple(difficulty, Achieve.AllMarvelous)];
    }
}
