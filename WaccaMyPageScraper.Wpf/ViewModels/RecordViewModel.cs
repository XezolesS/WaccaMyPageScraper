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
using WaccaMyPageScraper.FetcherActions;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.Wpf.Enums;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;
using System.Threading;
using WaccaMyPageScraper.Wpf.Views;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public sealed class RecordViewModel : FetcherViewModel
    {
        private RecordDetailModel RecordDetailModel;
        private RecordDetailWindow RecordDetailWindow;

        #region Properties
        private bool _filterDifficultyNormal;
        public bool FilterDifficultyNormal
        {
            get => _filterDifficultyNormal;
            set => SetProperty(ref _filterDifficultyNormal, value, 
                OnFilterAndSorterChanged);
        }

        private bool _filterDifficultyHard;
        public bool FilterDifficultyHard
        {
            get => _filterDifficultyHard;
            set => SetProperty(ref _filterDifficultyHard, value, 
                OnFilterAndSorterChanged);
        }

        private bool _filterDifficultyExpert;
        public bool FilterDifficultyExpert
        {
            get => _filterDifficultyExpert;
            set => SetProperty(ref _filterDifficultyExpert, value, 
                OnFilterAndSorterChanged);
        }

        private bool _filterDifficultyInferno;
        public bool FilterDifficultyInferno
        {
            get => _filterDifficultyInferno;
            set => SetProperty(ref _filterDifficultyInferno, value, 
                OnFilterAndSorterChanged);
        }

        private string _filterSearchText;
        public string FilterSearchText
        {
            get => _filterSearchText;
            set => SetProperty(ref _filterSearchText, value, 
                OnFilterAndSorterChanged);
        }

        public SortRecordBy[] SortOptions => Enum.GetValues<SortRecordBy>();

        private SortRecordBy _selectedSortBy;
        public SortRecordBy SelectedSortBy
        {
            get => _selectedSortBy;
            set => SetProperty(ref _selectedSortBy, value, 
                OnFilterAndSorterChanged);
        }

        private bool _isSortDescending;
        public bool IsSortDescending
        {
            get => _isSortDescending;
            set => SetProperty(ref _isSortDescending, value, 
                OnFilterAndSorterChanged);
        }

        private bool _isRichView;
        public bool IsRichView
        {
            get => _isRichView;
            set => SetProperty(ref _isRichView, value);
        }

        IEnumerable<RecordModel> _records;
        public IEnumerable<RecordModel> Records
        {
            get => _records;
            set => SetProperty(ref _records, value, 
                OnFilterAndSorterChanged);
        }

        ObservableCollection<RecordModel> _filteredRecords;
        public ObservableCollection<RecordModel> FilteredRecords
        {
            get => _filteredRecords;
            set => SetProperty(ref _filteredRecords, value);
        }

        public DelegateCommand FetchRecordsCommand { get; private set; }

        public DelegateCommand OpenRecordDetailCommand { get; private set; }
        #endregion

        public RecordViewModel(IEventAggregator ea) : base(ea)
        {
            this.RecordDetailWindow = new RecordDetailWindow(); // Initialize for it's ViewModel

            this.FilterDifficultyNormal = false;
            this.FilterDifficultyHard = false;
            this.FilterDifficultyExpert = true;
            this.FilterDifficultyInferno = false;

            this.FilterSearchText = string.Empty;

            this.FetchProgressText = WaccaMyPageScraper.Localization.Fetcher.LoggedOff;
            this.FetchProgressPercent = 0;

            this.FilteredRecords = new ObservableCollection<RecordModel>();
            this.SelectedSortBy = SortRecordBy.Default;
            this.IsSortDescending = false;

            this.IsRichView = true;

            this.FetchRecordsCommand = new DelegateCommand(FetcherEvent);
            this.OpenRecordDetailCommand = new DelegateCommand(OpenRecordDetailEvent);
        }

        public override async void InitializeData()
        {
            var musics = await new CsvHandler<Music>(Log.Logger)
                .ImportAsync(Directories.RecordData);

            var musicRankings = await new CsvHandler<MusicRankings>(Log.Logger)
                .ImportAsync(Directories.RecordRankings);

            if (musics is null)
                return;

            // Update a property
            this.Records = RecordModel.FromMusics(musics, musicRankings);
        }

        public override async void FetcherEvent()
        {
            if (this.fetcher is null)
                return;

            if (!this.IsFetchable)
                return;

            this.IsFetchable = false;

            // Create Directory
            if (!Directory.Exists(Directories.Record))
                Directory.CreateDirectory(Directories.Record);

            // Reset Records
            this.Records = new List<RecordModel>();
            this.FilteredRecords.Clear();

            // Fetch music list
            var musicMetadata = await this.fetcher.FetchMusicMetadataAsync(
                new Progress<string>(progressText => this.FetchProgressText = progressText),
                new Progress<int>(progressPercent => this.FetchProgressPercent = progressPercent));

            // Fetch records
            int count = 0;
            var musics = new List<Music>();
            var rankings = new List<MusicRankings>();
            foreach (var meta in musicMetadata)
            {
                musics.Add(await this.fetcher.FetchMusicAsync(
                    new Progress<string>(progressText => this.FetchProgressText = string.Format(
                        WaccaMyPageScraper.Localization.Fetcher.FetchingProgressMsg,
                        count, musicMetadata.Length, progressText)),
                    new Progress<int>(),
                    meta));
                rankings.Add(await this.fetcher.FetchMusicRankingsAsync(
                    new Progress<string>(progressText => this.FetchProgressText = string.Format(
                        WaccaMyPageScraper.Localization.Fetcher.FetchingProgressMsg,
                        count, musicMetadata.Length, progressText)),
                    new Progress<int>(),
                    meta));

                await this.fetcher.FetchMusicImageAsync(meta);
                ++count;

                this.FetchProgressText = string.Format(WaccaMyPageScraper.Localization.Fetcher.FetchingProgress,
                    count, musicMetadata.Length, meta.Title);
                this.FetchProgressPercent = (int)(((double)count / musicMetadata.Length) * 100);
            }

            // Fetch total score rankings
            var totalScoreRankings = await this.fetcher.FetchTotalScoreRankingsAsync(
                new Progress<string>(progressText => this.FetchProgressText = progressText),
                new Progress<int>(progressPercent => this.FetchProgressPercent = progressPercent));

            // Save records
            var musicCsvHandler = new CsvHandler<Music>(musics, Log.Logger);
            musicCsvHandler.Export(Directories.RecordData);

            var musicRankingsCsvHandler = new CsvHandler<MusicRankings>(rankings, Log.Logger);
            musicRankingsCsvHandler.Export(Directories.RecordRankings);

            var totalScoreRankingsCsvHandler = new CsvHandler<TotalScoreRankings>(
                new List<TotalScoreRankings> { totalScoreRankings }, Log.Logger);
            totalScoreRankingsCsvHandler.Export(Directories.TotalScoreRankings);

            // Convert Music to RecordModels
            this.Records = RecordModel.FromMusics(musics, rankings);

            // Set comlete message
            this.FetchProgressText = string.Format(WaccaMyPageScraper.Localization.Fetcher.DataFetched3,
                this.Records.Count(), WaccaMyPageScraper.Localization.Data.Record);

            this.IsFetchable = true;
        }
        
        public async void OpenRecordDetailEvent()
        {
            if (this.Records == null || this.Records.Count() == 0)
                return;

            // Reset window.
            this.RecordDetailWindow.Close();
            this.RecordDetailWindow = new RecordDetailWindow();

            // Build record detail model
            var totalScoreRankings = await new CsvHandler<TotalScoreRankings>(Log.Logger)
                .ImportAsync(Directories.TotalScoreRankings);
            this.RecordDetailModel = RecordDetailModel.ConvertFrom(this.Records.ToArray(), totalScoreRankings?.First());

            // Publish a event
            this._eventAggregator.GetEvent<OpenRecordDetailEvent>().Publish(this.RecordDetailModel);

            // Show record detail window
            this.RecordDetailWindow.ShowDialog();
        }

        private async void OnFilterAndSorterChanged()
        {
            if (this.Records is null)
                return;

            // Filtering
            var filtered = new List<RecordModel>();

            var asyncRecords = this.Records
                .ToAsyncEnumerable()
                .WhereAwait(record => FilterRecords(record))
                .ConfigureAwait(false);

            await foreach (var r in asyncRecords)
                filtered.Add(r);
            
            // Sorting
            IEnumerable<RecordModel> sorted = this.SelectedSortBy switch
            {
                SortRecordBy.Title => sorted = this.IsSortDescending ?
                    filtered.OrderByDescending(r => r.Title)
                    : filtered.OrderBy(r => r.Title),

                SortRecordBy.Artist => sorted = this.IsSortDescending ?
                    filtered.OrderByDescending(r => r.Artist)
                    : filtered.OrderBy(r => r.Artist),

                SortRecordBy.Level => sorted = this.IsSortDescending ?
                    filtered.OrderByDescending(r => r.LevelToNumber())
                    : filtered.OrderBy(r => r.LevelToNumber()),

                SortRecordBy.Score => sorted = this.IsSortDescending ?
                    filtered.OrderByDescending(r => r.Score)
                    : filtered.OrderBy(r => r.Score),

                SortRecordBy.PlayCount => sorted = this.IsSortDescending ?
                    filtered.OrderByDescending(r => r.PlayCount)
                    : filtered.OrderBy(r => r.PlayCount),

                _ => sorted = filtered,
            };

            this.FilteredRecords = new ObservableCollection<RecordModel>(sorted);
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
