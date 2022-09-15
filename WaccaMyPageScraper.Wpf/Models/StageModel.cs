using System.Collections.Generic;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.Models
{
    public sealed class StageModel : Stage
    {
        public byte[] StageIcon => ImageLocator.GetStageIcon(this);

        public StageData StageData => StageDataMap.StageDatas[this.Id];

        public StageModel() { }

        public StageModel(Stage stage) : base(stage.Id, stage.Name, stage.Grade, stage.Scores, stage.TotalScore) { }

        public static StageModel FromStage(Stage data) => new StageModel(data);

        public static IEnumerable<StageModel> FromStages(IEnumerable<Stage> data)
        {
            if (data is null)
                return null;

            var stages = new List<StageModel>();
            foreach (var stage in data)
                stages.Add(FromStage(stage));

            return stages;
        }
    }
}
