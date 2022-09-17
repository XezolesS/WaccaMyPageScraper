using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.Models
{
    public sealed class StageModel : Stage
    {
        public byte[] StageIcon => ImageLocator.GetStageIcon(this);

        public StageData StageData => StageDataMap.StageDatas[this.Id];

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

        public StageModel() { }

        public StageModel(Stage stage) : base(stage.Id, stage.Name, stage.Grade, stage.Scores, stage.TotalScore)
        {
            this.Ranking = -1;
        }

        public StageModel(Stage stage, StageRanking ranking) : base(stage.Id, stage.Name, stage.Grade, stage.Scores, stage.TotalScore) 
        {
            this.Ranking = ranking?.Ranking ?? -1;
        }

        public static StageModel FromStage(Stage data, StageRanking ranking) => new StageModel(data, ranking);

        public static IEnumerable<StageModel> FromStages(IEnumerable<Stage> data, IEnumerable<StageRanking> rankings)
        {
            if (data is null)
                return null;

            if (data.Count() == 0)
                return null;

            var stages = new List<StageModel>();
            foreach (var stage in data)
                stages.Add(FromStage(stage,
                    rankings?.Count() == 0 ? null : rankings?.First(r => r.Id == stage.Id)));

            return stages;
        }
    }
}
