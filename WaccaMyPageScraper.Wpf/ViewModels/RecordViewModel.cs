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
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.Fetchers;
using WaccaMyPageScraper.Resources;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public class RecordViewModel : BindableBase
    {
        private PageConnector pageConnector;

        private string _downloadStateText;
        public string DownloadStateText
        {
            get => _downloadStateText;
            set => SetProperty(ref _downloadStateText, value);
        }

        private int _musicCount;
        public int MusicCount
        {
            get => _musicCount;
            set => SetProperty(ref _musicCount, value);
        }

        private int _musicFetched;
        public int MusicFetched
        {
            get => _musicFetched;
            set => SetProperty(ref _musicFetched, value);
        }

        IList<RecordModel> _records;
        public IList<RecordModel> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }

        public DelegateCommand FetchRecordsCommand { get; private set; }

        public RecordViewModel(IEventAggregator ea)
        {
            InitializeDataFromFile();

            this.DownloadStateText = "Not Logged In";
            this.MusicCount = 1;
            this.MusicFetched = 0;

            this.FetchRecordsCommand = new DelegateCommand(ExcuteFetchRecordsCommand);

            ea.GetEvent<LoginSuccessEvent>().Subscribe(UpdatePageConnector);
        }

        private void InitializeDataFromFile()
        {
            var musicDetails = new CsvHandler<MusicDetail>(Log.Logger)
                .Import(DataFilePath.RecordData);

            this.Records = ConvertMusicDetailsToRecords(musicDetails);
        }

        private async void ExcuteFetchRecordsCommand()
        {
            if (pageConnector is null)
                return;

            // Fetch music list
            this.DownloadStateText = "Finding musics...";

            MusicsFetcher musicsFetcher = new MusicsFetcher(this.pageConnector);
            var musicList = await musicsFetcher.FetchAsync();

            this.MusicCount = musicList.Length;
            this.DownloadStateText = string.Format("Total {0} musics found.", this.MusicCount);

            // Fetch records
            var musicDetails = new List<MusicDetail>();
            MusicDetailFetcher musicDetailFetcher = new MusicDetailFetcher(this.pageConnector);
            foreach (var music in musicList)
            {
                musicDetails.Add(await musicDetailFetcher.FetchAsync(music));
                this.MusicFetched++;

                this.DownloadStateText = string.Format("({0}/{1}) Fetching {2}", 
                    this.MusicCount, this.MusicFetched, music.Title);
            }
            
            // Save records
            if (!Directory.Exists(DataFilePath.Record))
                Directory.CreateDirectory(DataFilePath.Record);

            var csvHandler = new CsvHandler<MusicDetail>(musicDetails, Log.Logger);
            csvHandler.Export(DataFilePath.RecordData);

            // Convert MusicDetails to RecordModels
            this.Records = ConvertMusicDetailsToRecords(musicDetails);

            this.DownloadStateText = "Download Complete";
        }

        private void UpdatePageConnector(PageConnector connector)
        {
            this.pageConnector = connector;

            if (connector.IsLoggedOn())
                this.DownloadStateText = "Logged In";
        }

        private IList<RecordModel> ConvertMusicDetailsToRecords(IEnumerable<MusicDetail> musicDetails)
        {
            var records = new List<RecordModel>();
            foreach (var musicDetail in musicDetails)
            {
                records.Add(RecordModel.FromMusicDetail(musicDetail, Difficulty.Normal));
                records.Add(RecordModel.FromMusicDetail(musicDetail, Difficulty.Hard));
                records.Add(RecordModel.FromMusicDetail(musicDetail, Difficulty.Expert));

                if (musicDetail.HasInferno())
                    records.Add(RecordModel.FromMusicDetail(musicDetail, Difficulty.Inferno));
            }

            return records;
        }
    }
}
