using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;

namespace WaccaMyPageScraper.Console.Options
{
    internal class ReadOption : Option
    {
        private string inputFilePath;

        public ReadOption(ILogger? logger, Fetcher fetcher, string parameter, string inputFilePath) 
            : base(logger, fetcher, parameter) 
        {
            this.inputFilePath = inputFilePath;
        }

        public override object Execute()
        {
            switch (this.parameter)
            {
                case "player":
                    {
                        var playerCsvHandler = new CsvHandler<Player>(Log.Logger);
                        var playerRecords = playerCsvHandler.Import(this.inputFilePath);

                        if (playerRecords is not null)
                            Log.Information("\n{Records}", playerRecords.First());

                        return playerRecords;
                    }
                case "record":
                    {
                        var musicCsvHandler = new CsvHandler<Music>(Log.Logger);
                        var musicRecords = musicCsvHandler.Import(this.inputFilePath);

                        if (musicRecords is not null)
                            Log.Information("\n{Records}", string.Join("\n", musicRecords));

                        return musicRecords;
                    }
                case "stage":
                    {
                        var stageCsvHandler = new CsvHandler<Stage>(Log.Logger);
                        var stageRecords = stageCsvHandler.Import(this.inputFilePath);

                        if (stageRecords is not null)
                            Log.Information("\n{Records}", string.Join("\n", stageRecords));

                        return stageRecords;
                    }
                case "trophy":
                    {
                        var trophyCsvHandler = new CsvHandler<Trophy>(Log.Logger);
                        var trophyRecords = trophyCsvHandler.Import(this.inputFilePath);

                        if (trophyRecords is not null)
                            Log.Information("\n{Records}", string.Join("\n", trophyRecords));

                        return trophyRecords;
                    }
                default:
                    this.logger?.Error("Invalid data information.");

                    return null;
            }
        }
    }
}
