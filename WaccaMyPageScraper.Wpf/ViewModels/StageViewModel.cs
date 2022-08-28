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

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public class StageViewModel : BindableBase
    {
        private PageConnector pageConnector;

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

        public StageViewModel(IEventAggregator ea)
        {
            InitializeDataFromFile();

            this.Stages = new List<StageModel>();

            this.DownloadStateText = "Not Logged In";
            this.StageCount = 1;
            this.StageFetched = 0;

            this.FetchStagesCommand = new DelegateCommand(ExcuteFetchStagesCommand);

            ea.GetEvent<LoginSuccessEvent>().Subscribe(UpdatePageConnector);
        }

        private async void InitializeDataFromFile()
        {
            var stageDetails = await new CsvHandler<StageDetail>(Log.Logger)
                .ImportAsync(DataFilePath.StageData);

            this.Stages = StageModel.FromStageDetails(stageDetails);
        }

        private async void ExcuteFetchStagesCommand()
        {
            if (pageConnector is null)
                return;

            // Reset Records
            this.Stages = new List<StageModel>();
            this.StageFetched = 0;

            // Fetch music list
            this.DownloadStateText = "Finding stages...";

            StagesFetcher stagesFetcher = new StagesFetcher(this.pageConnector);
            var stageList = await stagesFetcher.FetchAsync();

            this.StageCount = stageList.Length;
            this.DownloadStateText = string.Format("Total {0} stages found.", this.StageCount);

            // Fetch records
            var stageDetails = new List<StageDetail>();
            StageDetailFetcher stageDetailFetcher = new StageDetailFetcher(this.pageConnector);
            foreach (var stage in stageList)
            {
                stageDetails.Add(await stageDetailFetcher.FetchAsync(stage));

                this.StageFetched++;

                this.DownloadStateText = string.Format("({0}/{1}) Fetching {2}",
                    this.StageCount, this.StageFetched, stage.Name);
            }

            // Save records
            if (!Directory.Exists(DataFilePath.Stage))
                Directory.CreateDirectory(DataFilePath.Stage);

            // Convert MusicDetails to RecordModels
            this.Stages = StageModel.FromStageDetails(stageDetails);

            var csvHandler = new CsvHandler<StageDetail>(stageDetails, Log.Logger);
            csvHandler.Export(DataFilePath.StageData);

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
