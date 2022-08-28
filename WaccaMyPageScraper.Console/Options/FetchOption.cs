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

        public override object Execute()
        {
            switch (parameter.ToLower())
            {
                case "player":
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

                case "record":
                    {
                        var musicsFetcher = new MusicsFetcher(this.pageConnector);
                        var musics = Task.Run(async () => await musicsFetcher.FetchAsync()).Result;

                        int count = 0;
                        var musicDetails = new List<MusicDetail>();
                        foreach (var music in musics)
                        {
                            var musicDetailFetcher = new MusicDetailFetcher(this.pageConnector);
                            var musicDetail = Task.Run(async () => await musicDetailFetcher.FetchAsync(music)).Result;

                            musicDetails.Add(musicDetail);

                            Log.Information("{Count} out of {Musics} has been fetched", ++count, musics.Length);
                        }

                        if (this.outputFilePath is not null)
                        {
                            var musicDetailCsvHandler = new CsvHandler<MusicDetail>(musicDetails, Log.Logger);
                            musicDetailCsvHandler.Export(this.outputFilePath);
                        }

                        return musicDetails;
                    }

                case "stage":
                    {
                        var stagesFetcher = new StagesFetcher(pageConnector);
                        var stages = Task.Run(async () => await stagesFetcher.FetchAsync()).Result;

                        Log.Information("{Stages} of stages have been fetched", stages.Length);

                        int count = 0;
                        var stageDetails = new List<StageDetail>();
                        foreach (var stage in stages)
                        {
                            var stageDetailFetcher = new StageDetailFetcher(pageConnector);
                            var stageDetail = Task.Run(async () => await stageDetailFetcher.FetchAsync(stage)).Result;

                            stageDetails.Add(stageDetail);

                            Log.Information("{Count} out of {Musics} has been fetched", ++count, stages.Length, stageDetail);
                        } 
                        
                        if (this.outputFilePath is not null)
                        {
                            var stageDetailCsvHandler = new CsvHandler<StageDetail>(stageDetails, Log.Logger);
                            stageDetailCsvHandler.Export(this.outputFilePath);
                        }

                        return stageDetails;
                    }

                case "trophy":
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

                case "icon_rate":
                    {
                        var rateIconFetcher = new RateIconFetcher(pageConnector);
                        var result = Task.Run(async () => await rateIconFetcher.FetchAsync()).Result;

                        return result;
                    }

                case "icon_achieve":
                    {
                        var achieveIconFetcher = new AchieveIconFetcher(pageConnector);
                        var result = Task.Run(async () => await achieveIconFetcher.FetchAsync()).Result;

                        return result;
                    }

                case "icon_stage":
                    {
                        var stageIconFetcher = new StageIconFetcher(pageConnector);
                        var result = Task.Run(async () => await stageIconFetcher.FetchAsync()).Result;

                        return result;
                    }

                case "icon_trophy":
                    {
                        var trophyIconFetcher = new TrophyIconFetcher(pageConnector);
                        var result = Task.Run(async () => await trophyIconFetcher.FetchAsync()).Result;

                        return result;
                    }

                default:
                    this.logger?.Error("Invalid data information.");

                    return null;
            }
        }
    }
}
