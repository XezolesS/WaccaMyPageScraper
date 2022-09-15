using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.FetcherActions;

namespace WaccaMyPageScraper.Console.Options
{
    internal class FetchOption : Option
    {
        private readonly string? outputFilePath;

        public FetchOption(ILogger? logger, Fetcher fetcher, string parameter)
            : base(logger, fetcher, parameter)
        {
            this.outputFilePath = null;
        }

        public FetchOption(ILogger? logger, Fetcher fetcher, string parameter, string? outputFilePath)
            : base(logger, fetcher, parameter)
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
            "icon_trophy" => FetchTrophyIconTask(),
            _ => () => this.logger?.Error("Invalid data information.")
        };

        #region Fetch Tasks
        private Player FetchPlayerTask()
        {
            var player = Task.Run(async () => await this.fetcher.FetchPlayerAsync()).Result;

            if (this.outputFilePath is not null)
            {
                var csvHandler = new CsvHandler<Player>(new List<Player> { player }, Log.Logger);
                csvHandler.Export(this.outputFilePath);
            }

            return player;
        }

        private Music[] FetchRecordsTask()
        {
            var musicMetadata = Task.Run(async () => await this.fetcher.FetchMusicMetadataAsync()).Result;

            int count = 0;
            var musics = new List<Music>();
            foreach (var meta in musicMetadata)
            {
                var music = Task.Run(async () => await this.fetcher.FetchMusicAsync(meta)).Result;

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
            var stageMetadata = Task.Run(async () => await this.fetcher.FetchStageMetadataAsync()).Result;

            int count = 0;
            var stages = new List<Stage>();
            foreach (var meta in stageMetadata)
            {
                var stageDetail = Task.Run(async () => await this.fetcher.FetchStageAsync(meta)).Result;

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
            var trophies = Task.Run(async () => await this.fetcher.FetchTrophiesAsync()).Result;

            Log.Information("{Trophies} of trophies have been fetched", trophies.Length);

            if (this.outputFilePath is not null)
            {
                var trophyCsvHandler = new CsvHandler<Trophy>(trophies, Log.Logger);
                trophyCsvHandler.Export(this.outputFilePath);
            }

            return trophies;
        }

        private bool FetchRateIconTask() => Task.Run(async () => await this.fetcher.FetchRateIconsAsync()).Result;

        private bool FetchAchieveIconTask() => Task.Run(async () => await this.fetcher.FetchAchieveIconsAsync()).Result;

        private bool FetchStageIconTask() => Task.Run(async () => await this.fetcher.FetchStageIconsAsync()).Result;

        private bool FetchTrophyIconTask() => Task.Run(async () => await this.fetcher.FetchTrophyIconsAsync()).Result;
        #endregion
    }
}
