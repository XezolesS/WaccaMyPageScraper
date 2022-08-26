using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Wpf.Events;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private string _aimeId;
        public string AimeId
        {
            get => _aimeId;
            set => this.SetProperty(ref _aimeId, value);
        }
        
        public DelegateCommand LoginCommand { get; private set; }

        IEventAggregator _eventAggregator;

        public LoginViewModel(IEventAggregator ea)
        {
            this.LoginCommand = new DelegateCommand(ExecuteLoginCommand);

            this._eventAggregator = ea;
        }

        private void ExecuteLoginCommand()
        {
            var connector = new PageConnector(this.AimeId);
            var loginResult = Task.Run(async () => await connector.LoginAsync()).Result;

            if (loginResult)
            {
                this._eventAggregator.GetEvent<LoginSuccessEvent>().Publish(connector);
            }
        }
    }
}
