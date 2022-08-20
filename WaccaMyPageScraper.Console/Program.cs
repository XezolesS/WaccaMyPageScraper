// See https://aka.ms/new-console-template for more information
using WaccaMyPageScraper;
using WaccaMyPageScraper.Data;
using Microsoft.Extensions.Logging;
using Serilog;
using WaccaMyPageScraper.Fetchers;

// Create logger
Log.Logger = new LoggerConfiguration()
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
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("================================================");
    Console.WriteLine("Type a number of a task which you want to do.\n");
    Console.WriteLine("\t(1) Fetch player's data");
    Console.WriteLine("\t(2) Fetch player's music records");
    Console.WriteLine("\t(3) Fetch player's stage records");
    Console.WriteLine("\t(4) Fetch player's trophies");
    Console.WriteLine("\t(0) Exit");
    Console.WriteLine("================================================");

    Console.Write(" >> ");
    Console.ForegroundColor = ConsoleColor.White;

    string? input = Console.ReadLine();
    if (!int.TryParse(input, out int result))
    {
        Log.Warning("Input must be a integer value.");

        continue;
    }

    switch (int.Parse(input))
    {
        case 0: return;
        case 1: await LogPlayerAsync(); break;
        case 2: await LogMusicDetailsAsync(); break;
        case 3: await LogStageDetailAsync(); break;
        case 4: await LogTrophyDetailAsync(); break;
        default: break;
    }
}

Log.CloseAndFlush();
pageConnector.Dispose();

#region LocalFunctions
string GetAimeId()
{
    while (true)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Type your Aime ID >> ");
        Console.ForegroundColor = ConsoleColor.White;

        string? aimeId = Console.ReadLine();

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

async Task<Player> LogPlayerAsync()
{
    var playerFetcher = new PlayerFetcher(pageConnector);

    return await playerFetcher.FetchAsync();
}

async Task<Music[]> LogMusicDetailsAsync()
{
    var musicsFetcher = new MusicsFetcher(pageConnector);
    var musics = await musicsFetcher.FetchAsync();

    int count = 0;
    foreach (var music in musics)
    {
        var musicDetailFetcher = new MusicDetailFetcher(pageConnector);
        var musicDetail = await musicDetailFetcher.FetchAsync(music);

        Log.Information("{Count} out of {Musics} has been fetched", ++count, musics.Length);

        Thread.Sleep(100);
    }

    return musics;
}

async Task<Stage[]> LogStageDetailAsync()
{
    var stagesFetcher = new StagesFetcher(pageConnector);
    var stages = await stagesFetcher.FetchAsync();

    Log.Information("{Stages} of stages have been fetched", stages.Length);

    int count = 0;
    foreach (var stage in stages)
    {
        var stageDetailFetcher = new StageDetailFetcher(pageConnector);
        var stageDetail = await stageDetailFetcher.FetchAsync(stage);

        Log.Information("{Count} out of {Musics} has been fetched", ++count, stages.Length, stageDetail);

        Thread.Sleep(100);
    }

    return stages;
}

async Task<Trophy[]> LogTrophyDetailAsync()
{
    var trophiesFetcher = new TrophiesFetcher(pageConnector);
    var trophies = await trophiesFetcher.FetchAsync();

    Log.Information("{Trophies} of trophies have been fetched", trophies.Length);

    return trophies;
}
#endregion