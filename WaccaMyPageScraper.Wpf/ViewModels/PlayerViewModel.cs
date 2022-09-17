using Prism.Events;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.FetcherActions;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public sealed class PlayerViewModel : FetcherViewModel
    {
        private Player _player;
        public Player Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }

        public PlayerViewModel(IEventAggregator ea) : base()
        {
            ea.GetEvent<LoginSuccessEvent>().Subscribe(UpdatePlayerData);
        }

        public override async void InitializeData()
        {
            var playerRecords = await new CsvHandler<Player>(Log.Logger)
                .ImportAsync(Directories.PlayerData);

            var totalRpRanking = await new CsvHandler<TotalRpRanking>(Log.Logger)
                .ImportAsync(Directories.TotalRpRanking);

            if (playerRecords is null)
                return;

            var player = playerRecords.First();

            // Update properties
            this.Player = PlayerModel.FromPlayer(player, totalRpRanking?.First());
        }

        // There is no fether in this ViewModel.
        // UpdatePlayerData() will do the tasks instead.
        public override void FetcherEvent() => throw new NotImplementedException();

        private async void UpdatePlayerData(Fetcher fetcher)
        {
            var player = Task.Run(async () => await fetcher.FetchPlayerAsync()).Result;
            var totalRpRanking = Task.Run(async () => await fetcher.FetchTotalRpRankingFetch()).Result;

            var playerIcon = await fetcher.FetchPlayerIconAsync();

            // Save player
            if (!Directory.Exists(Directories.Player))
                Directory.CreateDirectory(Directories.Player);

            var playerCsvHandler = new CsvHandler<Player>(new List<Player> { player }, Log.Logger);
            playerCsvHandler.Export(Directories.PlayerData);

            var totalRpRankingCsvHandler = new CsvHandler<TotalRpRanking>(new List<TotalRpRanking> { totalRpRanking }, Log.Logger);
            totalRpRankingCsvHandler.Export(Directories.TotalRpRanking);

            // Update properties
            this.Player = PlayerModel.FromPlayer(player, totalRpRanking);
        }
    }
}
