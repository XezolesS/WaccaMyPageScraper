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
            var musicMetadata = await FetchMusicMetadataAsync();

            if (musicMetadata is null)
            {
                this.IsFetchable = true;
                return;
            }

            // Fetch records
            var records = await FetchRecordsAsync(musicMetadata);
            var musics = records.Item1;
            var rankings = records.Item2;

            if (musics is null || rankings is null ||
                musics.Last() is null || rankings.Last() is null)
            {
                this.IsFetchable = true;
                return;
            }

            // Fetch total score rankings
            var totalScoreRankings = await FetchTotalScoreRankingsAsync();

            // Save records
            var musicCsvHandler = new CsvHandler<Music>(musics, Log.Logger);
            musicCsvHandler.Export(Directories.RecordData);

            var musicRankingsCsvHandler = new CsvHandler<MusicRankings>(rankings, Log.Logger);
            musicRankingsCsvHandler.Export(Directories.RecordRankings);

            if (totalScoreRankings is not null)
            {
                var totalScoreRankingsCsvHandler = new CsvHandler<TotalScoreRankings>(
                    new List<TotalScoreRankings> { totalScoreRankings }, Log.Logger);
                totalScoreRankingsCsvHandler.Export(Directories.TotalScoreRankings);
            }

            // Convert Music to RecordModels
            this.Records = RecordModel.FromMusics(musics, rankings);

            // Set comlete message
            this.FetchProgressText = string.Format(WaccaMyPageScraper.Localization.Fetcher.DataFetched3,
                this.Records.Count(), WaccaMyPageScraper.Localization.Data.Record);

            this.IsFetchable = true;
        }

        private async Task<MusicMetadata[]?> FetchMusicMetadataAsync()
        {
            try
            {
                return await this.fetcher.FetchMusicMetadataAsync(
                    new Progress<string>(progressText => this.FetchProgressText = progressText),
                    new Progress<int>(progressPercent => this.FetchProgressPercent = progressPercent));
            }
            catch (Exception ex)
            {
                if (!await this.fetcher.TryLoginAsync(
                        new Progress<string>(progressText =>
                            this.FetchProgressText = progressText)))
                {
                    Log.Error(WaccaMyPageScraper.Localization.Fetcher.FetchingFail + " {0}",
                            WaccaMyPageScraper.Localization.Data.MusicMetadata, ex.Message);

                    this.FetchProgressText = string.Format(WaccaMyPageScraper.Localization.Fetcher.FetchingFail,
                        WaccaMyPageScraper.Localization.Data.MusicMetadata);
                    this.FetchProgressPercent = 0;

                    return null;
                }

                return await FetchMusicMetadataAsync();
            }
        }

        private async Task<(Music[]?, MusicRankings[]?)> FetchRecordsAsync(MusicMetadata[] musicMetadata)
        {
            int count = 0;

            var musics = new List<Music>();
            var rankings = new List<MusicRankings>();
            for (int i = 0; i < musicMetadata.Length; i++)
            {
                var meta = musicMetadata[i];

                try
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
                catch (Exception ex)
                {
                    i--;

                    if (!await this.fetcher.TryLoginAsync(
                        new Progress<string>(progressText => 
                            this.FetchProgressText = progressText)))
                    {
                        Log.Error(WaccaMyPageScraper.Localization.Fetcher.FetchingFail + " {0}",
                            WaccaMyPageScraper.Localization.Data.Music, ex.Message);

                        this.FetchProgressText = string.Format(WaccaMyPageScraper.Localization.Fetcher.FetchingFail,
                            WaccaMyPageScraper.Localization.Data.Music);
                        this.FetchProgressPercent = 0;

                        return (null, null);
                    }
                }
            }

            return (musics.ToArray(), rankings.ToArray());
        }

        private async Task<TotalScoreRankings?> FetchTotalScoreRankingsAsync()
        {
            try
            {
                return await this.fetcher.FetchTotalScoreRankingsAsync(
                    new Progress<string>(progressText => this.FetchProgressText = progressText),
                    new Progress<int>(progressPercent => this.FetchProgressPercent = progressPercent));
            }
            catch (Exception ex)
            {
                if (!await this.fetcher.TryLoginAsync(
                        new Progress<string>(progressText =>
                            this.FetchProgressText = progressText)))
                {
                    Log.Error(WaccaMyPageScraper.Localization.Fetcher.FetchingFail + " {0}",
                     WaccaMyPageScraper.Localization.Data.TotalScoreRanking, ex.Message);

                    this.FetchProgressText = string.Format(WaccaMyPageScraper.Localization.Fetcher.FetchingFail,
                        WaccaMyPageScraper.Localization.Data.TotalScoreRanking);
                    this.FetchProgressPercent = 0;

                    return null;
                }

                return await FetchTotalScoreRankingsAsync();
            }
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
