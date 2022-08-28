using Prism.Events;
using Prism.Mvvm;
using Serilog;
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
    public class PlayerViewModel : BindableBase
    {
        private Player _player;
        public Player Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }

        private byte[] _playerIcon;
        public byte[] PlayerIcon
        {
            get => _playerIcon;
            set => this.SetProperty(ref _playerIcon, value);
        }

        private byte[] _stageIcon;
        public byte[] StageIcon
        {
            get => _stageIcon;
            set => this.SetProperty(ref _stageIcon, value);
        }

        public PlayerViewModel(IEventAggregator ea)
        {
            InitializeDataFromFile();

            ea.GetEvent<LoginSuccessEvent>().Subscribe(UpdatePlayerData);
        }
        private async void InitializeDataFromFile()
        {
            var playerRecords = await new CsvHandler<Player>(Log.Logger)
                .ImportAsync(DataFilePath.PlayerData);
            var player = playerRecords.First();

            this.Player = PlayerModel.FromPlayer(player);

            this.PlayerIcon = GetImageByte(Path.GetFullPath(DataFilePath.PlayerIcon));
            this.StageIcon = GetImageByte(Path.GetFullPath(DataFilePath.PlayerStageIcon));
        }

        private async void UpdatePlayerData(PageConnector connector)
        {
            PlayerFetcher fetcher = new PlayerFetcher(connector);
            var player = Task.Run(async () => await fetcher.FetchAsync()).Result;

            var csvHandler = new CsvHandler<Player>(new List<Player> { player }, Log.Logger);
            csvHandler.Export(DataFilePath.PlayerData);

            var playerIcon = await fetcher.FetchPlayerIconAsync();

            // Update properties
            this.Player = PlayerModel.FromPlayer(player);

            this.PlayerIcon = GetImageByte(playerIcon);
            this.StageIcon = ImageLocator.LocateStage(player.Stage);
        }

        private byte[] GetImageByte(string? DataFilePath)
        {
            if (!File.Exists(DataFilePath))
                return null;

            byte[] image = null;
            using (var fs = new FileStream(DataFilePath, FileMode.Open, FileAccess.Read))
            {
                image = new byte[fs.Length];
                while(fs.Read(image, 0, image.Length) > 0)
                {
                    image[image.Length - 1] = (byte)fs.ReadByte();
                }
            }

            return image;
        }
    }
}
