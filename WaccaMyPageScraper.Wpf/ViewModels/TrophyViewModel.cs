using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class TrophyViewModel : BindableBase
    {
        private PageConnector pageConnector;

        private string[] _seasons;
        public string[] Seasons => new string[3]
        {
            "Season 1", "Season 2", "Season 3"
        };

        private string _selectedSeason;
        public string SelectedSeason
        {
            get => _selectedSeason;
            set => SetProperty(ref _selectedSeason, value, OnSeasonChanged);
        }

        private string[] _clearRates;
        public string[] ClearRates
        {
            get => _clearRates;
            set => SetProperty(ref _clearRates, value);
        }

        public byte[][] TrophyRarityIcons => new byte[4][]
        {
            ImageLocator.LocateTrophyBronze(),
            ImageLocator.LocateTrophySilver(),
            ImageLocator.LocateTrophyGold(),
            ImageLocator.LocateTrophyPlatinum(),
        };

        private string _downloadStateText;
        public string DownloadStateText
        {
            get => _downloadStateText;
            set => SetProperty(ref _downloadStateText, value);
        }

        private int _trophyCount;
        public int TrophyCount
        {
            get => _trophyCount;
            set => SetProperty(ref _trophyCount, value);
        }

        private int _trophyFetched;
        public int TrophyFetched
        {
            get => _trophyFetched;
            set => SetProperty(ref _trophyFetched, value);
        }

        private IEnumerable<TrophyModel> _trophies;
        public IEnumerable<TrophyModel> Trophies
        {
            get => _trophies;
            set => SetProperty(ref _trophies, value, OnSeasonChanged);
        }

        ObservableCollection<TrophyModel> _filteredTrophies;
        public ObservableCollection<TrophyModel> FilteredTrophies
        {
            get => _filteredTrophies;
            set => SetProperty(ref _filteredTrophies, value);
        }

        public DelegateCommand FetchTrophiesCommand { get; private set; }

        public TrophyViewModel(IEventAggregator ea)
        {
            InitializeDataFromFile();

            this.SelectedSeason = "Season 1";

            this.DownloadStateText = "Not Logged In";
            this.TrophyCount = 1;
            this.TrophyFetched = 0;

            this.FetchTrophiesCommand = new DelegateCommand(ExcuteFetchTrophiesCommand);

            ea.GetEvent<LoginSuccessEvent>().Subscribe(UpdatePageConnector);
        }

        private void OnSeasonChanged()
        {
            if (this.Trophies is null)
                return;

            var filtered = _trophies.Where(t => 
                (this.SelectedSeason.Equals("Season 1") && t.Id.ToString().StartsWith("10"))
                || (this.SelectedSeason.Equals("Season 2") && t.Id.ToString().StartsWith("20"))
                || (this.SelectedSeason.Equals("Season 3") && t.Id.ToString().StartsWith("30")));

            var bronzes = filtered.Where(t => t.Rarity == 1);
            var silver = filtered.Where(t => t.Rarity == 2);
            var gold = filtered.Where(t => t.Rarity == 3);
            var platinum = filtered.Where(t => t.Rarity == 4);

            var clearRates = new string[4];
            clearRates[0] = string.Format("{0}/{1}", 
                bronzes.Where(t => t.Obtained).Count(), bronzes.Count());
            clearRates[1] = string.Format("{0}/{1}", 
                silver.Where(t => t.Obtained).Count(), silver.Count());
            clearRates[2] = string.Format("{0}/{1}", 
                gold.Where(t => t.Obtained).Count(), gold.Count());
            clearRates[3] = string.Format("{0}/{1}", 
                platinum.Where(t => t.Obtained).Count(), platinum.Count());

            this.FilteredTrophies = new ObservableCollection<TrophyModel>(filtered);
            this.ClearRates = clearRates;
        }

        private async void InitializeDataFromFile()
        {
            var trophies = await new CsvHandler<Trophy>(Log.Logger)
                .ImportAsync(DataFilePath.TrophyData);

            this.Trophies = TrophyModel.FromTrophies(trophies);
        }

        private async void ExcuteFetchTrophiesCommand()
        {
            if (pageConnector is null)
                return;

            // Reset Trophies
            this.Trophies = new List<TrophyModel>();

            // Fetch trophies
            this.DownloadStateText = "Fetching trophies...";

            TrophiesFetcher trophiesFetcher = new TrophiesFetcher(this.pageConnector);
            var trophies = await trophiesFetcher.FetchAsync();

            this.TrophyCount = trophies.Length;
            this.DownloadStateText = string.Format("Total {0} trophies fetched.", this.TrophyCount);

            // Save trophies
            if (!Directory.Exists(DataFilePath.Trophy))
                Directory.CreateDirectory(DataFilePath.Trophy);

            var csvHandler = new CsvHandler<Trophy>(trophies, Log.Logger);
            csvHandler.Export(DataFilePath.TrophyData);

            // Convert Trophies to TrophyModels
            this.Trophies = TrophyModel.FromTrophies(trophies);

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
