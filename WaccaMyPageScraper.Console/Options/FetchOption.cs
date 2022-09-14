using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Fetchers;

namespace WaccaMyPageScraper.Console.Options
{
    internal class FetchOption : Option
    {
        private readonly string? outputFilePath;

        public FetchOption(ILogger? logger, PageConnector pageConnector, string parameter)
            : base(logger, pageConnector, parameter)
        {
            this.outputFilePath = null;
        }

        public FetchOption(ILogger? logger, PageConnector pageConnector, string parameter, string? outputFilePath)
            : base(logger, pageConnector, parameter)
        {
            this.outputFilePath = outputFilePath;
        }

        public override object Execute() => this.parameter.ToLower() switch {
            "player" => FetchPlayerTask(),
            "record" => FetchRecordsTask(),
            "stage" => FetchStagesTask(),
            "trophy" => FetchTrophiesTask(),
            "icon_rate" => FetchRateIconTask(),
            "icon_achieve" => FetchAchieveIconTask(),
            "icon_stage" => FetchStageIconTask(),
            "icon_trophy" => FetchStageIconTask(),
            _ => () => this.logger?.Error("Invalid data information.")
        };

        #region Fetch Tasks
        private Player FetchPlayerTask()
        {
            var playerFetcher = new PlayerFetcher(this.pageConnector);
            var player = Task.Run(async () => await playerFetcher.FetchAsync()).Result;

            if (this.outputFilePath is not null)
            {
                var csvHandler = new CsvHandler<Player>(new List<Player> { player }, Log.Logger);
                csvHandler.Export(this.outputFilePath);
            }

            return player;
        }

        private Music[] FetchRecordsTask()
        {
            var musicMetadataFetcher = new MusicMetadataFetcher(this.pageConnector);
            var musicMetadata = Task.Run(async () => await musicMetadataFetcher.FetchAsync()).Result;

            int count = 0;
            var musics = new List<Music>();
            foreach (var meta in musicMetadata)
            {
                var musicFetcher = new MusicFetcher(this.pageConnector);
                var music = Task.Run(async () => await musicFetcher.FetchAsync(meta)).Result;

                musics.Add(music);

                Log.Information("{Count} out of {Musics} has been fetched", ++count, musicMetadata.Length);
            }

            if (this.outputFilePath is not null)
            {
                var musicCsvHandler = new CsvHandler<Music>(musics, Log.Logger);
                musicCsvHandler.Export(this.outputFilePath);
            }

            return musics.ToArray();
        }

        private Stage[] FetchStagesTask()
        {
            var stageMetadataFetcher = new StageMetadataFetcher(pageConnector);
            var stageMetadata = Task.Run(async () => await stageMetadataFetcher.FetchAsync()).Result;

            int count = 0;
            var stages = new List<Stage>();
            foreach (var meta in stageMetadata)
            {
                var stageFetcher = new StageFetcher(pageConnector);
                var stageDetail = Task.Run(async () => await stageFetcher.FetchAsync(meta)).Result;

                stages.Add(stageDetail);

                Log.Information("{Count} out of {Musics} has been fetched", ++count, stageMetadata.Length, stageDetail);
            }

            if (this.outputFilePath is not null)
            {
                var stageDetailCsvHandler = new CsvHandler<Stage>(stages, Log.Logger);
                stageDetailCsvHandler.Export(this.outputFilePath);
            }

            return stages.ToArray();
        }

        private Trophy[] FetchTrophiesTask()
        {
            var trophiesFetcher = new TrophiesFetcher(pageConnector);
            var trophies = Task.Run(async () => await trophiesFetcher.FetchAsync()).Result;

            Log.Information("{Trophies} of trophies have been fetched", trophies.Length);

            if (this.outputFilePath is not null)
            {
                var trophyCsvHandler = new CsvHandler<Trophy>(trophies, Log.Logger);
                trophyCsvHandler.Export(this.outputFilePath);
            }

            return trophies;
        }

        private bool FetchRateIconTask() => 
            Task.Run(async () => await new RateIconFetcher(this.pageConnector).FetchAsync()).Result;

        private bool FetchAchieveIconTask() =>
            Task.Run(async () => await new AchieveIconFetcher(this.pageConnector).FetchAsync()).Result;

        private bool FetchStageIconTask() =>
            Task.Run(async () => await new StageIconFetcher(this.pageConnector).FetchAsync()).Result;

        private bool FetchTrophyIconTask() =>
            Task.Run(async () => await new TrophyIconFetcher(this.pageConnector).FetchAsync()).Result;
        #endregion
    }
}
