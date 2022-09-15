﻿using Prism.Commands;
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
using WaccaMyPageScraper.FetcherActions;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public sealed class TrophyViewModel : FetcherViewModel
    {
        #region Properties
        private string[] _seasons;
        public string[] Seasons => new string[3]
        {
            $"{WaccaMyPageScraper.Localization.Data.Season} 1", 
            $"{WaccaMyPageScraper.Localization.Data.Season} 2", 
            $"{WaccaMyPageScraper.Localization.Data.Season} 3"
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
            ImageLocator.GetTrophyBronzeIcon(),
            ImageLocator.GetTrophySilverIcon(),
            ImageLocator.GetTrophyGoldIcon(),
            ImageLocator.GetTrophyPlatinumIcon(),
        };

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
        #endregion

        public TrophyViewModel(IEventAggregator ea) : base(ea)
        {
            this.SelectedSeason = "Season 1";

            this.FetchProgressText = "Not Logged In";
            this.FetchProgressPercent = 0;

            this.FetchTrophiesCommand = new DelegateCommand(FetcherEvent);
        }

        public override async void InitializeData()
        {
            var trophies = await new CsvHandler<Trophy>(Log.Logger)
                .ImportAsync(Directories.TrophyData);

            this.Trophies = TrophyModel.FromTrophies(trophies);
        }

        public override async void FetcherEvent()
        {
            if (this.fetcher is null)
                return;

            // Reset Trophies
            this.Trophies = new List<TrophyModel>();

            // Fetch trophies
            var trophies = await this.fetcher.FetchTrophiesAsync(
                new Progress<string>(progressText => this.FetchProgressText = progressText),
                new Progress<int>(progressPercent => this.FetchProgressPercent = progressPercent));

            // Save trophies
            if (!Directory.Exists(Directories.Trophy))
                Directory.CreateDirectory(Directories.Trophy);

            var csvHandler = new CsvHandler<Trophy>(trophies, Log.Logger);
            csvHandler.Export(Directories.TrophyData);

            // Convert Trophies to TrophyModels
            this.Trophies = TrophyModel.FromTrophies(trophies);
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
    }
}
