using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Resources;
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

        public bool IsAbleToAccess
        {
           get => DateTime.Compare(DateTime.Now, new DateTime(2022, 9, 30)) < 0;
        }
        
        public DelegateCommand LoginCommand { get; private set; }

        IEventAggregator _eventAggregator;

        public LoginViewModel(IEventAggregator ea)
        {
            this.AimeId = ReadAimeFile();

            this.LoginCommand = new DelegateCommand(ExecuteLoginCommand);

            this._eventAggregator = ea;
        }

        private async void ExecuteLoginCommand()
        {
            var connector = new PageConnector(this.AimeId, Log.Logger);
            var loginResult = await connector.LoginAsync();

            if (loginResult)
            {
                this._eventAggregator.GetEvent<LoginSuccessEvent>().Publish(connector);
                WriteAimeFile();
            }
        }

        private void WriteAimeFile()
        {
            if (!Directory.Exists(DataFilePath.Root))
                Directory.CreateDirectory(DataFilePath.Root);
            

            using (var fs = new FileStream(DataFilePath.AimeId, FileMode.Create, FileAccess.Write))
                fs.Write(Encoding.UTF8.GetBytes(this.AimeId));
        }

        private string ReadAimeFile()
        {
            if (!File.Exists(DataFilePath.AimeId))
                return null;

            string result = null;
            using (var fs = new FileStream(DataFilePath.AimeId, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[fs.Length];

                while (fs.Read(buffer, 0, buffer.Length) > 0)
                    result = Encoding.UTF8.GetString(buffer);
            }

            return result;
        }
    }
}
