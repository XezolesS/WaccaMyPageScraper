﻿using Prism.Commands;
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
    public sealed class LoginViewModel : FetcherViewModel
    {
        private string _aimeId;
        public string AimeId
        {
            get => _aimeId;
            set => this.SetProperty(ref _aimeId, value);
        }

        public bool IsAbleToAccess
        {
            get => DateTime.Compare(
               DateTime.Now.ToUniversalTime().AddHours(9), // to UTC+9
               new DateTime(2022, 9, 30, 23, 59, 59)) <= 0;
        }
        
        public DelegateCommand LoginCommand { get; private set; }

        public LoginViewModel(IEventAggregator ea) : base(ea)
        {
            this.LoginCommand = new DelegateCommand(FetcherEvent);
        }

        public override void InitializeData()
        {
            // Read AimeId from the file.
            if (!File.Exists(Directories.AimeId))
                return;

            string aimeId = null;
            using (var fs = new FileStream(Directories.AimeId, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[fs.Length];

                while (fs.Read(buffer, 0, buffer.Length) > 0)
                    aimeId = Encoding.UTF8.GetString(buffer);
            }

            // Update AimeId
            this.AimeId = aimeId;
        }

        public override async void FetcherEvent()
        {
            var fetcher = new Fetcher(this.AimeId, Log.Logger);
            var loginResult = await fetcher.LoginAsync();

            if (loginResult)
            {
                this._eventAggregator.GetEvent<LoginSuccessEvent>().Publish(fetcher);

                // Save AimeId as a text file.
                if (!Directory.Exists(Directories.Root))
                    Directory.CreateDirectory(Directories.Root);

                using (var fs = new FileStream(Directories.AimeId, FileMode.Create, FileAccess.Write))
                    fs.Write(Encoding.UTF8.GetBytes(this.AimeId));
            }
        }
    }
}
