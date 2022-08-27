using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.Fetchers;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public class RecordViewModel : BindableBase
    {
        private PageConnector pageConnector;

        private bool _filterDifficultyNormal;
        public bool FilterDifficultyNormal
        {
            get => _filterDifficultyNormal;
            set => SetProperty(ref _filterDifficultyNormal, value, OnFilterChanged);
        }

        private bool _filterDifficultyHard;
        public bool FilterDifficultyHard
        {
            get => _filterDifficultyHard;
            set => SetProperty(ref _filterDifficultyHard, value, OnFilterChanged);
        }

        private bool _filterDifficultyExpert;
        public bool FilterDifficultyExpert
        {
            get => _filterDifficultyExpert;
            set => SetProperty(ref _filterDifficultyExpert, value, OnFilterChanged);
        }

        private bool _filterDifficultyInferno;
        public bool FilterDifficultyInferno
        {
            get => _filterDifficultyInferno;
            set => SetProperty(ref _filterDifficultyInferno, value, OnFilterChanged);
        }

        private string _filterSearchText;
        public string FilterSearchText
        {
            get => _filterSearchText;
            set => SetProperty(ref _filterSearchText, value, OnFilterChanged);
        }

        private string _downloadStateText;
        public string DownloadStateText
        {
            get => _downloadStateText;
            set => SetProperty(ref _downloadStateText, value);
        }

        private int _musicCount;
        public int MusicCount
        {
            get => _musicCount;
            set => SetProperty(ref _musicCount, value);
        }

        private int _musicFetched;
        public int MusicFetched
        {
            get => _musicFetched;
            set => SetProperty(ref _musicFetched, value);
        }

        IEnumerable<RecordModel>? _records;
        public IEnumerable<RecordModel>? Records
        {
            get => _records?
                .Where(r => (this.FilterDifficultyNormal && r.Difficulty == Difficulty.Normal)
                        || (this.FilterDifficultyHard && r.Difficulty == Difficulty.Hard)
                        || (this.FilterDifficultyExpert && r.Difficulty == Difficulty.Expert)
                        || (this.FilterDifficultyInferno && r.Difficulty == Difficulty.Inferno))
                .Where(r => r.Title.Contains(this.FilterSearchText, StringComparison.CurrentCultureIgnoreCase) 
                        || r.Artist.Contains(this.FilterSearchText, StringComparison.CurrentCultureIgnoreCase));
            set => SetProperty(ref _records, value);
        }

        public DelegateCommand FetchRecordsCommand { get; private set; }

        public RecordViewModel(IEventAggregator ea)
        {
            InitializeDataFromFile();

            this.FilterDifficultyNormal = false;
            this.FilterDifficultyHard = false;
            this.FilterDifficultyExpert = false;
            this.FilterDifficultyInferno = false;

            this.FilterSearchText = string.Empty;

            this.DownloadStateText = "Not Logged In";
            this.MusicCount = 1;
            this.MusicFetched = 0;

            this.FetchRecordsCommand = new DelegateCommand(ExcuteFetchRecordsCommand);

            ea.GetEvent<LoginSuccessEvent>().Subscribe(UpdatePageConnector);
        }

        private void OnFilterChanged()
        {
            this.Records = this.Records;
        }

        private async void InitializeDataFromFile()
        {
            var musicDetails = await new CsvHandler<MusicDetail>(Log.Logger)
                .ImportAsync(DataFilePath.RecordData);

            this.Records = ConvertAllMusicDetailsToRecords(musicDetails);
        }

        private async void ExcuteFetchRecordsCommand()
        {
            if (pageConnector is null)
                return;

            // Reset Records
            this.Records = new List<RecordModel>();

            // Fetch music list
            this.DownloadStateText = "Finding musics...";

            MusicsFetcher musicsFetcher = new MusicsFetcher(this.pageConnector);
            var musicList = await musicsFetcher.FetchAsync();

            this.MusicCount = musicList.Length;
            this.DownloadStateText = string.Format("Total {0} musics found.", this.MusicCount);

            // Fetch records
            var musicDetails = new List<MusicDetail>();
            MusicDetailFetcher musicDetailFetcher = new MusicDetailFetcher(this.pageConnector);
            foreach (var music in musicList)
            {
                musicDetails.Add(await musicDetailFetcher.FetchAsync(music));
                await musicDetailFetcher.FetchMusicImageAsync(music.Id);

                this.MusicFetched++;

                this.DownloadStateText = string.Format("({0}/{1}) Fetching {2}", 
                    this.MusicCount, this.MusicFetched, music.Title);
            }
            
            // Save records
            if (!Directory.Exists(DataFilePath.Record))
                Directory.CreateDirectory(DataFilePath.Record);

            // Convert MusicDetails to RecordModels
            this.Records = ConvertAllMusicDetailsToRecords(musicDetails);

            var csvHandler = new CsvHandler<MusicDetail>(musicDetails, Log.Logger);
            csvHandler.Export(DataFilePath.RecordData);

            this.DownloadStateText = "Download Complete";
        }

        private void UpdatePageConnector(PageConnector connector)
        {
            this.pageConnector = connector;

            if (connector.IsLoggedOn())
                this.DownloadStateText = "Logged In";
        }

        private IList<RecordModel> ConvertAllMusicDetailsToRecords(IEnumerable<MusicDetail> musicDetails)
        {
            var records = new List<RecordModel>();
            foreach (var musicDetail in musicDetails)
            {
                records.Add(RecordModel.FromMusicDetail(musicDetail, Difficulty.Normal));
                records.Add(RecordModel.FromMusicDetail(musicDetail, Difficulty.Hard));
                records.Add(RecordModel.FromMusicDetail(musicDetail, Difficulty.Expert));

                if (musicDetail.HasInferno())
                    records.Add(RecordModel.FromMusicDetail(musicDetail, Difficulty.Inferno));
            }

            return records;
        }
    }
}
