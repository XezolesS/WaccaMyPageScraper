// See https://aka.ms/new-console-template for more information
using WaccaMyPageScraper;
using WaccaMyPageScraper.Data;
using Microsoft.Extensions.Logging;
using Serilog;
using WaccaMyPageScraper.Fetchers;

// Create logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

PageConnector pageConnector;

// Retreive Aime Id
while (true)
{
    var aimeId = GetAimeId();
    var isLoginSuccess = await LoginToPageAsync(aimeId);

    if (isLoginSuccess)
    {
        Log.Information("Login succeed!");
        break;
    }

    Log.Error("Login failed!");
}

var playerFetcher = new PlayerFetcher(pageConnector);
var player = await playerFetcher.FetchAsync();
Log.Information("${PlayerData}", player);

var musicFetcher = new MusicsFetcher(pageConnector);
var musics = await musicFetcher.FetchAsync();
foreach (var music in musics)
{
    Log.Information("{Music}", music);
}

var musicDetailFetcher = new MusicDetailFetcher(pageConnector);
var musicDetailTest = await musicDetailFetcher.FetchAsync(musics[0].Id);
Log.Information("{MusicDetail}", musicDetailTest);

Log.CloseAndFlush();
pageConnector.Dispose();

#region LocalFunctions
string GetAimeId()
{
    string aimeId;
    while (true)
    {
        Console.Write("Type your Aime ID >> ");
        aimeId = Console.ReadLine();

        if (string.IsNullOrEmpty(aimeId))
            Log.Error("The given ID is not valid!");
        else 
            return aimeId;
    }
}

async Task<bool> LoginToPageAsync(string aimeId)
{
    bool result = false;

    pageConnector = new PageConnector(aimeId, Log.Logger);
    result = await pageConnector.LoginAsync();
    
    return result;
}
#endregion