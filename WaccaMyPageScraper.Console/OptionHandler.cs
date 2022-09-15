using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Console.Options;

namespace WaccaMyPageScraper.Console
{
    internal class OptionHandler
    {
        private readonly ILogger? _logger;

        private Fetcher fetcher;

        public static readonly IDictionary<string, string> CommandMapping = new Dictionary<string, string>()
        {
            { "-u", "Id" },
            { "--userid", "Id" },

            { "-f", "Fetch" },
            { "--fetch", "Fetch" },

            { "-r", "Read" },
            { "--read", "Read" },

            { "-o", "Output" },
            { "-i", "Input" },

            { "-h", "Help" },
            { "--help", "Help" },
        };

        public IConfigurationRoot Config { get; private set; }

        public OptionHandler(ILogger _logger, IConfigurationRoot config)
        {
            this._logger = _logger;
            this.Config = config;
        }

        public void Run()
        {
            if (IsCommandNull() || Config["Help"] is not null)
            {
                var helpOption = new HelpOption(_logger, fetcher, Config["Help"]);

                helpOption.Execute();
                return;
            }

            if (IsReadAndFetch())
            {
                _logger?.Error("Fetch and Read options cannot be used at the same time!");

                return;
            }

            // Run fetch option
            if (Config["Fetch"] is not null)
            {
                if (Config["Id"] is null)
                {
                    _logger?.Error("Id is required to fetch data!");

                    return;
                }

                var idOption = new IdOption(_logger, fetcher, Config["Id"]);
                this.fetcher = idOption.Execute() as Fetcher;

                var outputOption = new OutputOption(_logger, fetcher, Config["Output"]);
                var outputFilePath = outputOption.Execute() as string;

                var fetchOption = new FetchOption(_logger, this.fetcher, Config["Fetch"], outputFilePath);
                fetchOption.Execute();

                return;
            }

            // Run read option
            if (Config["Read"] is not null)
            {
                if (Config["Input"] is null)
                {
                    _logger?.Error("No file input to read!");

                    return;
                }

                var inputOption = new InputOption(_logger, fetcher, Config["Input"]);
                var inputFilePath = inputOption.Execute() as string;

                var readOption = new ReadOption(_logger, fetcher, Config["Read"], inputFilePath);
                readOption.Execute();

                return;
            }
        }

        private bool IsCommandNull() => Config["Id"] is null
            && Config["Fetch"] is null
            && Config["Read"] is null
            && Config["Help"] is null;

        private bool IsReadAndFetch() => Config["Fetch"] is not null && Config["Read"] is not null;
    }
}
