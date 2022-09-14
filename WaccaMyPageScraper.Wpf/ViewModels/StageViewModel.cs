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
using WaccaMyPageScraper.Fetchers;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public sealed class StageViewModel : FetcherViewModel
    {
        private PageConnector pageConnector;

        #region Properties
        private string _downloadStateText;
        public string DownloadStateText
        {
            get => _downloadStateText;
            set => SetProperty(ref _downloadStateText, value);
        }

        private int _stageCount;
        public int StageCount
        {
            get => _stageCount;
            set => SetProperty(ref _stageCount, value); 
        }

        private int _stageFetched;
        public int StageFetched
        {
            get => _stageFetched;
            set => SetProperty(ref _stageFetched, value);
        }

        private IEnumerable<StageModel> _stages;
        public IEnumerable<StageModel> Stages
        {
            get => _stages;
            set => SetProperty(ref _stages, value);
        }

        public DelegateCommand FetchStagesCommand { get; private set; }
        #endregion

        public StageViewModel(IEventAggregator ea) : base()
        {
            this.Stages = new List<StageModel>();

            this.DownloadStateText = "Not Logged In";
            this.StageCount = 1;
            this.StageFetched = 0;

            this.FetchStagesCommand = new DelegateCommand(FetcherEvent);

            ea.GetEvent<LoginSuccessEvent>().Subscribe(UpdatePageConnector);
        }

        public override async void InitializeData()
        {
            StageDataMap.Initialize(); // Initialize stage data.

            var stages = await new CsvHandler<Stage>(Log.Logger)
                .ImportAsync(DataFilePath.StageData);

            this.Stages = StageModel.FromStages(stages);
        }

        public override async void FetcherEvent()
        {
            if (pageConnector is null)
                return;

            // Reset Stages
            this.Stages = new List<StageModel>();
            this.StageFetched = 0;

            // Fetch stage list
            this.DownloadStateText = "Finding stages...";

            StageMetadataFetcher stageMetadataFetcher = new StageMetadataFetcher(this.pageConnector);
            var stageMetadata = await stageMetadataFetcher.FetchAsync();

            this.StageCount = stageMetadata.Length;
            this.DownloadStateText = string.Format("Total {0} stages found.", this.StageCount);

            // Fetch stages
            var stages = new List<Stage>();
            StageFetcher stageDetailFetcher = new StageFetcher(this.pageConnector);
            foreach (var meta in stageMetadata)
            {
                stages.Add(await stageDetailFetcher.FetchAsync(meta));

                this.StageFetched++;

                this.DownloadStateText = string.Format("({0}/{1}) Fetching {2}",
                    this.StageFetched, this.StageCount, meta.Name);
            }

            // Save stages
            if (!Directory.Exists(DataFilePath.Stage))
                Directory.CreateDirectory(DataFilePath.Stage);

            var csvHandler = new CsvHandler<Stage>(stages, Log.Logger);
            csvHandler.Export(DataFilePath.StageData);

            // Convert Stage to StageModels
            this.Stages = StageModel.FromStages(stages);

            this.DownloadStateText = "Download Complete";
        }

        private void UpdatePageConnector(PageConnector connector)
        {
            this.pageConnector = connector;

            if (connector.IsLoggedOn())
                this.DownloadStateText = "Logged In";
        }
    }
}
