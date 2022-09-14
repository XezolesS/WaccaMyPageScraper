using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaccaMyPageScraper.Wpf.Events;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    /// <summary>
    /// An abstract class of all fetcher releated ViewModels.
    /// </summary>
    public abstract class FetcherViewModel : BindableBase
    {
        protected PageConnector pageConnector;

        private string _fetchProgressText;
        public string FetchProgressText
        {
            get => _fetchProgressText;
            set => SetProperty(ref _fetchProgressText, value);
        }

        private int _fetchProgressPercent;
        public int FetchProgressPercent
        {
            get => _fetchProgressPercent;
            set => SetProperty(ref _fetchProgressPercent, value);
        }

        private Visibility _fetcherVisibility;
        public Visibility FetcherVisibility
        {
            get => _fetcherVisibility;
            set => SetProperty(ref _fetcherVisibility, value);
        }

        public FetcherViewModel() => InitializeData();

        public FetcherViewModel(IEventAggregator ea)
        {
            InitializeData();

            this.FetcherVisibility = Visibility.Collapsed;

            ea.GetEvent<LoginSuccessEvent>().Subscribe(UpdatePageConnector);
        }
        /// <summary>
        /// Initialize data and might be processed or showed.
        /// </summary>
        public abstract void InitializeData();

        /// <summary>
        /// An event callback interface for fetcher event.
        /// </summary>
        public abstract void FetcherEvent();

        private void UpdatePageConnector(PageConnector connector)
        {
            this.pageConnector = connector;

            if (connector.IsLoggedOn())
            {
                this.FetchProgressText = WaccaMyPageScraper.Localization.Connector.LoggedIn;
                this.FetcherVisibility = Visibility.Visible;
            }
            else
            {
                this.FetchProgressText = WaccaMyPageScraper.Localization.Connector.LoggedOff;
                this.FetcherVisibility = Visibility.Collapsed;
            }
        }
    }
}
