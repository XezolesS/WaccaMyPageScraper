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

        public Brush RankingColor => RankingColors.GetColor(this.Ranking);

        public string LocalizedName => 1 <= this.Id && this.Id <= 14 ?
            this.Name.Replace("ステージ", WaccaMyPageScraper.Localization.Data.Stage)
            : this.Name;

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
