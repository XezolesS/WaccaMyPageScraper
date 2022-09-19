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
using WaccaMyPageScraper.FetcherActions;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public sealed class StageViewModel : FetcherViewModel
    {
        #region Properties
        private IEnumerable<StageModel> _stages;
        public IEnumerable<StageModel> Stages
        {
            get => _stages;
            set => SetProperty(ref _stages, value);
        }

        public DelegateCommand FetchStagesCommand { get; private set; }
        #endregion

        public StageViewModel(IEventAggregator ea) : base(ea)
        {
            this.Stages = new List<StageModel>();

            this.FetchProgressText = WaccaMyPageScraper.Localization.Fetcher.LoggedOff;
            this.FetchProgressPercent = 0;

            this.FetchStagesCommand = new DelegateCommand(FetcherEvent);
        }

        public override async void InitializeData()
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

        public override async void FetcherEvent()
        {
            if (this.fetcher is null)
                return;

            if (!this.IsFetchable)
                return;

            this.IsFetchable = false;

            // Create Directory
            if (!Directory.Exists(Directories.Stage))
                Directory.CreateDirectory(Directories.Stage);

            // Reset Stages
            this.Stages = new List<StageModel>();

            // Fetch stage list
            var stageMetadata = await this.fetcher.FetchStageMetadataAsync(
                new Progress<string>(progressText => this.FetchProgressText = progressText),
                new Progress<int>(progressPercent => this.FetchProgressPercent = progressPercent));

            // Fetch stages
            int count = 0;
            var stages = new List<Stage>();
            var rankings = new List<StageRanking>();
            foreach (var meta in stageMetadata)
            {
                stages.Add(await this.fetcher.FetchStageAsync(
                    new Progress<string>(progressText => this.FetchProgressText = string.Format(
                        WaccaMyPageScraper.Localization.Fetcher.FetchingProgressMsg,
                        count, stageMetadata.Length, progressText)),
                    new Progress<int>(),
                    meta));
                rankings.Add(await this.fetcher.FetchStageRankingAsync(
                    new Progress<string>(progressText => this.FetchProgressText = string.Format(
                        WaccaMyPageScraper.Localization.Fetcher.FetchingProgressMsg,
                        count, stageMetadata.Length, progressText)),
                    new Progress<int>(),
                    meta));

                ++count;

                this.FetchProgressText = string.Format(WaccaMyPageScraper.Localization.Fetcher.FetchingProgress,
                    count, stageMetadata.Length, meta.Name);
                this.FetchProgressPercent = (int)(((double)count / stageMetadata.Length) * 100);
            }

            // Save stages
            var stageCsvHandler = new CsvHandler<Stage>(stages, Log.Logger);
            stageCsvHandler.Export(Directories.StageData);

            var stageRankingCsvHandler = new CsvHandler<StageRanking>(rankings, Log.Logger);
            stageRankingCsvHandler.Export(Directories.StageRankings);

            // Convert Stage to StageModels
            this.Stages = StageModel.FromStages(stages, rankings);

            // Set complete message
            this.FetchProgressText = string.Format(WaccaMyPageScraper.Localization.Fetcher.DataFetched3,
                this.Stages.Count(), WaccaMyPageScraper.Localization.Data.Stage);

            this.IsFetchable = true;
        }
    }
}
