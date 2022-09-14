using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Console.Options
{
    internal class HelpOption : Option
    {
        public HelpOption(ILogger? _logger, PageConnector _pageConnector, string _parameter) 
            : base(_logger, _pageConnector, _parameter) { }

        public override object Execute()
        {
            var helpTextBuilder = new StringBuilder();
            helpTextBuilder.AppendLine("\nOptions: ");

            helpTextBuilder.AppendLine("\t(-h | --help)");
            helpTextBuilder.AppendLine("\t\tPrint this texts.");
            helpTextBuilder.AppendLine();

            helpTextBuilder.AppendLine("\t(-u | --userid) AIME_ID");
            helpTextBuilder.AppendLine("\t\tSet Aime ID to login to the WACCA My Page.");
            helpTextBuilder.AppendLine();

            helpTextBuilder.AppendLine("\t(-f | --fetch) DATA");
            helpTextBuilder.AppendLine("\t\tFetch selected data from the WACCA My Page.");
            helpTextBuilder.AppendLine("\t\tIt needs Aime ID to login to the page.");
            helpTextBuilder.AppendLine("\t\tYou can save it to .csv file with a parameter \"-o\"");
            helpTextBuilder.AppendLine();

            helpTextBuilder.AppendLine("\t-o FILENAME");
            helpTextBuilder.AppendLine("\t\tSet output file name.");
            helpTextBuilder.AppendLine();

            helpTextBuilder.AppendLine("\t(-r | --read) DATA");
            helpTextBuilder.AppendLine("\t\tRead selected data from the .csv file.");
            helpTextBuilder.AppendLine("\t\tMust be used with a parameter \"-i\"");
            helpTextBuilder.AppendLine();

            helpTextBuilder.AppendLine("\t-i FILENAME");
            helpTextBuilder.AppendLine("\t\tSet input file name.");
            helpTextBuilder.AppendLine();

            helpTextBuilder.AppendLine("DATA: ");
            helpTextBuilder.AppendLine("\tPlayer\t\tPlayer stats(level, rates, play counts, etc.)");
            helpTextBuilder.AppendLine("\tRecord\t\tAll of the records about musics that the player has played.");
            helpTextBuilder.AppendLine("\tStage\t\tAll of the stage records that the player has passed.");
            helpTextBuilder.AppendLine("\tTrophy\t\tAll of the trophies that the player has obtained.");
            helpTextBuilder.AppendLine();
            helpTextBuilder.AppendLine("\tIcon_Rate\t\tClear rate icon images(D ~ MASTER).");
            helpTextBuilder.AppendLine("\tIcon_Achieve\t\tClear achieve level icon images(C ~ AM).");
            helpTextBuilder.AppendLine("\tIcon_Stage\t\tStage icons.");
            helpTextBuilder.AppendLine("\tIcon_Trophy\t\tTrophy icons.");
            helpTextBuilder.AppendLine();

            logger?.Information(helpTextBuilder.ToString());

            return null;
        }
    }
}
