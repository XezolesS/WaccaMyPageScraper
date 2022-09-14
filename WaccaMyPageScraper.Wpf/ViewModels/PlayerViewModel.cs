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
using WaccaMyPageScraper.Fetchers;
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
                .ImportAsync(DataFilePath.PlayerData);

            if (playerRecords is null)
                return;

            var player = playerRecords.First();

            // Update properties
            this.Player = PlayerModel.FromPlayer(player);
        }

        // There is no fether in this ViewModel.
        // UpdatePlayerData() will do the tasks instead.
        public override void FetcherEvent() => throw new NotImplementedException();

        private async void UpdatePlayerData(PageConnector connector)
        {
            PlayerFetcher fetcher = new PlayerFetcher(connector);
            var player = Task.Run(async () => await fetcher.FetchAsync()).Result;

            // Save player
            if (!Directory.Exists(DataFilePath.Player))
                Directory.CreateDirectory(DataFilePath.Player);

            var csvHandler = new CsvHandler<Player>(new List<Player> { player }, Log.Logger);
            csvHandler.Export(DataFilePath.PlayerData);

            var playerIcon = await fetcher.FetchPlayerIconAsync();

            // Update properties
            this.Player = PlayerModel.FromPlayer(player);
        }
    }
}
