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

        public ReadOption(ILogger? logger, PageConnector pageConnector, string parameter, string inputFilePath) 
            : base(logger, pageConnector, parameter) 
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
                        var musicDetailCsvHandler = new CsvHandler<MusicDetail>(Log.Logger);
                        var musicDetailRecords = musicDetailCsvHandler.Import(this.inputFilePath);

                        if (musicDetailRecords is not null)
                            Log.Information("\n{Records}", string.Join("\n", musicDetailRecords));

                        return musicDetailRecords;
                    }
                case "stage":
                    {
                        var stageDetailCsvHandler = new CsvHandler<StageDetail>(Log.Logger);
                        var stageDetailRecords = stageDetailCsvHandler.Import(this.inputFilePath);

                        if (stageDetailRecords is not null)
                            Log.Information("\n{Records}", string.Join("\n", stageDetailRecords));

                        return stageDetailRecords;
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
