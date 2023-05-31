using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public sealed class StageViewModel : BindableBase
    {
        #region Properties
        private IEnumerable<StageModel> _stages;
        public IEnumerable<StageModel> Stages
        {
            get => _stages;
            set => SetProperty(ref _stages, value);
        }
        #endregion

        public StageViewModel()
        {
            InitializeData();

            this.Stages = new List<StageModel>();
        }

        public async void InitializeData()
        {
            StageDataMap.Initialize(); // Initialize stage data.

            var stages = await new CsvHandler<Stage>(Log.Logger)
                .ImportAsync(Directories.StageData);

            var stageRankings = await new CsvHandler<StageRanking>(Log.Logger)
                .ImportAsync(Directories.StageRankings);

            if (stages is null)
                return;

            this.Stages = StageModel.FromStages(stages, stageRankings);
        }
    }
}
