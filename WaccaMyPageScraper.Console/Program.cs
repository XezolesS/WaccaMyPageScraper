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
int loginTries = 0;
while (true)
{
    if (loginTries >= 10)
    {
        Log.Error("Can not login to WACCA my page. Check your internet connection");

        return;
    }

    var aimeId = GetAimeId();
    var isLoginSuccess = await LoginToPageAsync(aimeId);

    if (isLoginSuccess)
    {
        Log.Information("Login succeed!");

        break;
    }

    Log.Error("Login failed!");
    loginTries++;
}

while (true)
{
    Console.WriteLine("Type a number of a task which you want to do.\n");
    Console.WriteLine("\t(1) Fetch player's data");
    Console.WriteLine("\t(2) Fetch player's music records");
    Console.WriteLine("\t(3) Fetch player's stage records");
    Console.WriteLine("\t(0) Exit");
    Console.WriteLine("");

    Console.Write(" >> ");
    string input = Console.ReadLine();
    if (!int.TryParse(input, out int result))
    {
        Log.Warning("Input must be a integer value.");

        continue;
    }

    switch (int.Parse(input))
    {
        case 0: return;
        case 1: Log.Information("${PlayerData}", await FetchPlayerAsync()); break;
        case 2: await LogMusicDetailsAsync(); break;
        case 3: await LogStageDetailAsync(); break;
        default: break;
    }
}

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

async Task<Player> FetchPlayerAsync()
{
    var playerFetcher = new PlayerFetcher(pageConnector);

    return await playerFetcher.FetchAsync(); ;
}

async Task<Music[]> LogMusicDetailsAsync()
{
    var musicFetcher = new MusicsFetcher(pageConnector);
    var musics = await musicFetcher.FetchAsync();

    Log.Information("{Musics} of musics have been fetched", musics.Length);

    int count = 0;
    foreach (var music in musics)
    {
        var musicDetailFetcher = new MusicDetailFetcher(pageConnector);
        var musicDetail = await musicDetailFetcher.FetchAsync(music.Id, music.Genre);

        Log.Information("{Count} out of {Musics} has been fetched: \n{MusicDetail}", ++count, musics.Length, musicDetail);

        Thread.Sleep(100);
    }

    return musics;
}

async Task<Stage[]> LogStageDetailAsync()
{
    var stageFetcher = new StagesFetcher(pageConnector);
    var stages = await stageFetcher.FetchAsync();

    Log.Information("{Stages} of stages have been fetched", stages.Length);

    return stages;
}
#endregion