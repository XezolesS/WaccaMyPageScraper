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

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public class PlayerViewModel : BindableBase
    {
        private string _playerName;
        public string PlayerName
        {
            get => _playerName;
            set => this.SetProperty(ref _playerName, value);
        }

        private string _playerLevel;
        public string PlayerLevel
        {
            get => "Lv." + _playerLevel;
            set => this.SetProperty(ref _playerLevel, value);
        }

        private string _playerRate;
        public string PlayerRate
        {
            get => "Rate " + _playerRate;
            set => this.SetProperty(ref _playerRate, value);
        }

        private byte[] _playerIconPath;
        public byte[] PlayerIconPath
        {
            get => _playerIconPath;
            set => this.SetProperty(ref _playerIconPath, value);
        }

        private byte[] _stageIconPath;
        public byte[] StageIconPath
        {
            get => _stageIconPath;
            set => this.SetProperty(ref _stageIconPath, value);
        }

        public PlayerViewModel(IEventAggregator ea)
        {
            InitializeDataFromFile();

            ea.GetEvent<LoginSuccessEvent>().Subscribe(UpdatePlayerData);
        }
        private void InitializeDataFromFile()
        {
            var player = new CsvHandler<Player>(Log.Logger)
                .Import(DataFilePath.PlayerData)?
                .First();

            this.PlayerName = player?.Name;
            this.PlayerLevel = player?.Level.ToString();
            this.PlayerRate = player?.Rate.ToString();

            this.PlayerIconPath = GetImageByte(Path.GetFullPath(DataFilePath.PlayerIcon));
            this.StageIconPath = GetImageByte(Path.GetFullPath(DataFilePath.PlayerStageIcon));
        }

        private async void UpdatePlayerData(PageConnector connector)
        {
            PlayerFetcher fetcher = new PlayerFetcher(connector);
            var player = Task.Run(async () => await fetcher.FetchAsync()).Result;

            var csvHandler = new CsvHandler<Player>(new List<Player> { player }, Log.Logger);
            csvHandler.Export(DataFilePath.PlayerData);

            var playerIcon = await fetcher.FetchPlayerIconAsync();
            var stageIcon = await fetcher.FetchStageIconAsync();

            // Update properties
            this.PlayerName = player?.Name;
            this.PlayerLevel = player?.Level.ToString();
            this.PlayerRate = player?.Rate.ToString();

            this.PlayerIconPath = GetImageByte(playerIcon);
            this.StageIconPath = GetImageByte(stageIcon);
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
