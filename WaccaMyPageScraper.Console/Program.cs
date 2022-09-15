// See https://aka.ms/new-console-template for more information
using WaccaMyPageScraper;
using WaccaMyPageScraper.Data;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.Extensions.Configuration;
using WaccaMyPageScraper.Console;

// Create logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var config = new ConfigurationBuilder()
    .AddCommandLine(args, OptionHandler.CommandMapping)
    .Build();

Log.Logger.Information("{Id}, {Fetch}, {Read}", config["Id"], config["Fetch"], config["Read"]);

var commandHandler = new OptionHandler(Log.Logger, config);
commandHandler.Run();

Log.CloseAndFlush();