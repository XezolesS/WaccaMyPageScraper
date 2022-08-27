using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Fetchers;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.Wpf.Enums;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;
using System.Threading;

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

        public SortRecordBy[] SortOptions => Enum.GetValues<SortRecordBy>();

        private SortRecordBy _selectedSortBy;
        public SortRecordBy SelectedSortBy
        {
            get => _selectedSortBy;
            set => SetProperty(ref _selectedSortBy, value, OnSortChanged);
        }

        private bool _isSortDescending;
        public bool IsSortDescending
        {
            get => _isSortDescending;
            set => SetProperty(ref _isSortDescending, value, OnSortChanged);
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

        IEnumerable<RecordModel> _records;
        public IEnumerable<RecordModel> Records
        {
            get => _records;
            set => SetProperty(ref _records, value, OnFilterChanged);
        }

        ObservableCollection<RecordModel> _filteredRecords;
        public ObservableCollection<RecordModel> FilteredRecords
        {
            get => _filteredRecords;
            set => SetProperty(ref _filteredRecords, value);
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

            this.FilteredRecords = new ObservableCollection<RecordModel>();
            this.IsSortDescending = false;

            this.FetchRecordsCommand = new DelegateCommand(ExcuteFetchRecordsCommand);

            ea.GetEvent<LoginSuccessEvent>().Subscribe(UpdatePageConnector);
        }

        private async void OnFilterChanged()
        {
            if (this.Records is null)
                return;

            this.FilteredRecords.Clear();

            var asyncRecords = this.Records
                .ToAsyncEnumerable()
                .WhereAwait(record => FilterRecords(record))
                .ConfigureAwait(false);

            await foreach (var r in asyncRecords)
                this.FilteredRecords.Add(r);
            
            if (this.SelectedSortBy != SortRecordBy.Default)
                OnSortChanged();
        }

        private async void OnSortChanged()
        {
            IEnumerable<RecordModel> sorted = null;
            switch (this.SelectedSortBy)
            {
                case SortRecordBy.Title:
                    sorted = this.IsSortDescending ?
                        this.FilteredRecords.OrderByDescending(r => r.Title)
                        : this.FilteredRecords.OrderBy(r => r.Title);

                    this.FilteredRecords = new ObservableCollection<RecordModel>(sorted); 
                    break;
                case SortRecordBy.Artist:
                        sorted = this.IsSortDescending ? 
                            this.FilteredRecords.OrderByDescending(r => r.Artist) 
                            : this.FilteredRecords.OrderBy(r => r.Artist);

                    this.FilteredRecords = new ObservableCollection<RecordModel>(sorted);
                    break;
                case SortRecordBy.Level:
                    sorted = this.IsSortDescending ?
                        this.FilteredRecords.OrderByDescending(r => r.LevelToNumber())
                        : this.FilteredRecords.OrderBy(r => r.LevelToNumber());

                    this.FilteredRecords = new ObservableCollection<RecordModel>(sorted);
                    break;
                case SortRecordBy.Score:
                    sorted = this.IsSortDescending ?
                        this.FilteredRecords.OrderByDescending(r => r.Score)
                        : this.FilteredRecords.OrderBy(r => r.Score);

                    this.FilteredRecords = new ObservableCollection<RecordModel>(sorted);
                    break;
                case SortRecordBy.PlayCount:
                    sorted = this.IsSortDescending ?
                        this.FilteredRecords.OrderByDescending(r => r.PlayCount)
                        : this.FilteredRecords.OrderBy(r => r.PlayCount);

                    this.FilteredRecords = new ObservableCollection<RecordModel>(sorted);
                    break;


                default: OnFilterChanged(); break;
            }
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
            this.FilteredRecords.Clear();

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

        private async ValueTask<bool> FilterRecords(RecordModel record) => 
            await FilterDifficultyAsync(record) && await FilterTextAsync(record);

        private async ValueTask<bool> FilterDifficultyAsync(RecordModel record) =>
            (this.FilterDifficultyNormal && record.Difficulty == Difficulty.Normal)
            || (this.FilterDifficultyHard && record.Difficulty == Difficulty.Hard)
            || (this.FilterDifficultyExpert && record.Difficulty == Difficulty.Expert)
            || (this.FilterDifficultyInferno && record.Difficulty == Difficulty.Inferno);

        private async ValueTask<bool> FilterTextAsync(RecordModel record) =>
            record.Title.Contains(this.FilterSearchText, StringComparison.CurrentCultureIgnoreCase)
            || record.Artist.Contains(this.FilterSearchText, StringComparison.CurrentCultureIgnoreCase);
    }
}
