using Prism.Events;
using Prism.Mvvm;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            get => _playerLevel;
            set => this.SetProperty(ref _playerLevel, value);
        }

        private string _playerRate;
        public string PlayerRate
        {
            get => _playerRate;
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
            ea.GetEvent<LoginSuccessEvent>().Subscribe(GetPlayerData);

            this.PlayerIconPath = GetImageByte(Path.GetFullPath(ImagePath.PlayerIcon));
            this.StageIconPath = GetImageByte(Path.GetFullPath(ImagePath.PlayerStageIcon));
        }

        void GetPlayerData(PageConnector connector)
        {
            PlayerFetcher fetcher = new PlayerFetcher(connector);
            var player = Task.Run(async () => await fetcher.FetchAsync()).Result;

            var playerIcon = Task.Run(async () => await fetcher.FetchPlayerIconAsync()).Result;
            var stageIcon = Task.Run(async () => await fetcher.FetchStageIconAsync()).Result;

            this.PlayerName = player.Name;
            this.PlayerLevel = "Lv." + player.Level.ToString();
            this.PlayerRate = "Rate " + player.Rate.ToString();

            this.PlayerIconPath = GetImageByte(playerIcon);
            this.StageIconPath = GetImageByte(stageIcon);
        }

        byte[] GetImageByte(string? imagePath)
        {
            if (!File.Exists(imagePath))
                return null;

            byte[] image = null;
            using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
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
