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
            "totalscore_ranking" => FetchTotalScoreRanking(),
            "totalrp_ranking" => FetchTotalRpRanking(),
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

        private MusicMetadata[] FetchRecordsTask()
        {
            var musicMetadata = Task.Run(async () => await this.fetcher.FetchMusicMetadataAsync()).Result;

            int count = 0;
            var musics = new List<Music>();
            var musicRankings = new List<MusicRankings>();
            foreach (var meta in musicMetadata)
            {
                var music = Task.Run(async () => await this.fetcher.FetchMusicAsync(meta)).Result;
                var musicRanking = Task.Run(async () => await this.fetcher.FetchMusicRankingsAsync(meta)).Result;

                musics.Add(music);
                musicRankings.Add(musicRanking);
            }

            if (this.outputFilePath is not null)
            {
                var musicCsvHandler = new CsvHandler<Music>(musics, Log.Logger);
                musicCsvHandler.Export(this.outputFilePath);

                var musicRankingsCsvHandler = new CsvHandler<MusicRankings>(musicRankings, Log.Logger);
                musicRankingsCsvHandler.Export(this.outputFilePath.Replace(".csv", "_rankings.csv"));
            }

            return musicMetadata.ToArray();
        }

        private Stage[] FetchStagesTask()
        {
            var stageMetadata = Task.Run(async () => await this.fetcher.FetchStageMetadataAsync()).Result;

            int count = 0;
            var stages = new List<Stage>();
            var stagesRankings = new List<StageRanking>();
            foreach (var meta in stageMetadata)
            {
                var stageDetail = Task.Run(async () => await this.fetcher.FetchStageAsync(meta)).Result;
                var stageRanking = Task.Run(async () => await this.fetcher.FetchStageRankingAsync(meta)).Result;

                stages.Add(stageDetail);
                stagesRankings.Add(stageRanking);
            }

            if (this.outputFilePath is not null)
            {
                var stageDetailCsvHandler = new CsvHandler<Stage>(stages, Log.Logger);
                stageDetailCsvHandler.Export(this.outputFilePath);

                var stageRankingCsvHandler = new CsvHandler<StageRanking>(stagesRankings, Log.Logger);
                stageRankingCsvHandler.Export(this.outputFilePath.Replace(".csv", "_rankings.csv"));
            }

            return stages.ToArray();
        }

        private Trophy[] FetchTrophiesTask()
        {
            var trophies = Task.Run(async () => await this.fetcher.FetchTrophiesAsync()).Result;

            if (this.outputFilePath is not null)
            {
                var trophyCsvHandler = new CsvHandler<Trophy>(trophies, Log.Logger);
                trophyCsvHandler.Export(this.outputFilePath);
            }

            return trophies;
        }

        private TotalScoreRankings FetchTotalScoreRanking()
        {
            var totalScoreRankings = Task.Run(async () => await this.fetcher.FetchTotalScoreRankingsFetch()).Result;

            if (this.outputFilePath is not null)
            {
                var totalScoreRankingsCsvHandler = new CsvHandler<TotalScoreRankings>(
                    new List<TotalScoreRankings> { totalScoreRankings }, Log.Logger);
                totalScoreRankingsCsvHandler.Export(this.outputFilePath);
            }

            return totalScoreRankings;
        }

        private TotalRpRanking FetchTotalRpRanking()
        {
            var totalRpRanking = Task.Run(async () => await this.fetcher.FetchTotalRpRankingFetch()).Result;

            if (this.outputFilePath is not null)
            {
                var totalRpRankingCsvHandler = new CsvHandler<TotalRpRanking>(
                    new List<TotalRpRanking> { totalRpRanking }, Log.Logger);
                totalRpRankingCsvHandler.Export(this.outputFilePath);
            }

            return totalRpRanking;
        }

        private string FetchRateIconTask() => Task.Run(async () => await this.fetcher.FetchRateIconsAsync()).Result;

        private string FetchAchieveIconTask() => Task.Run(async () => await this.fetcher.FetchAchieveIconsAsync()).Result;

        private string FetchStageIconTask() => Task.Run(async () => await this.fetcher.FetchStageIconsAsync()).Result;

        private string FetchTrophyIconTask() => Task.Run(async () => await this.fetcher.FetchTrophyIconsAsync()).Result;
        #endregion
    }
}
