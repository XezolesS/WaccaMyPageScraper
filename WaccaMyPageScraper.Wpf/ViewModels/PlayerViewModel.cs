using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Fetchers;
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

        public PlayerViewModel(IEventAggregator ea)
        {
            ea.GetEvent<LoginSuccessEvent>().Subscribe(GetPlayerData);
        }

        void GetPlayerData(PageConnector connector)
        {
            PlayerFetcher fetcher = new PlayerFetcher(connector);
            var player = Task.Run(async () => await fetcher.FetchAsync()).Result;

            this.PlayerName = player.Name;
            this.PlayerLevel = "Lv." + player.Level.ToString();
            this.PlayerRate = "Rate " + player.Rate.ToString();
        }
    }
}
