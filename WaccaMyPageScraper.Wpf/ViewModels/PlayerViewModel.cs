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
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;
using WaccaMyPageScraper.Wpf.Resources;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public sealed class PlayerViewModel : BindableBase
    {
        private Player _player;
        public Player Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }

        public PlayerViewModel()
        {
            InitializeData();
        }

        public async void InitializeData()
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
    }
}
