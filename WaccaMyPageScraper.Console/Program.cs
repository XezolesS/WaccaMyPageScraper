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

        case 5:
            var playerCsvHandler = new CsvHandler<Player>(Log.Logger);
            var playerRecords = playerCsvHandler.Import("player.csv");

            if (playerRecords is not null)
                Log.Information("\n{Records}", playerRecords.First());

            var musicCsvHandler = new CsvHandler<Music>(Log.Logger);
            var musicRecords = musicCsvHandler.Import("musics.csv");

            if (musicRecords is not null)
                Log.Information("\n{Records}", string.Join("\n", musicRecords));

            var musicDetailCsvHandler = new CsvHandler<MusicDetail>(Log.Logger);
            var musicDetailRecords = musicDetailCsvHandler.Import("music_details.csv");

            if (musicDetailRecords is not null)
                Log.Information("\n{Records}", string.Join("\n", musicDetailRecords));

            var stageCsvHandler = new CsvHandler<Stage>(Log.Logger);
            var stageRecords = stageCsvHandler.Import("stages.csv");

            if (stageRecords is not null)
                Log.Information("\n{Records}", string.Join("\n", stageRecords));

            var stageDetailCsvHandler = new CsvHandler<StageDetail>(Log.Logger);
            var stageDetailRecords = stageCsvHandler.Import("stage_details.csv");

            if (stageDetailRecords is not null)
                Log.Information("\n{Records}", string.Join("\n", stageDetailRecords));

            break;
        
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
    var player = await playerFetcher.FetchAsync();

    var csvHandler = new CsvHandler<Player>(new List<Player> { player }, Log.Logger);
    csvHandler.Export(Directory.GetCurrentDirectory(), "player.csv");

    return player;
}

async Task<Music[]> LogMusicDetailsAsync()
{
    var musicsFetcher = new MusicsFetcher(pageConnector);
    var musics = await musicsFetcher.FetchAsync();

    int count = 0;
    var musicDetails = new List<MusicDetail>();
    foreach (var music in musics)
    {
        var musicDetailFetcher = new MusicDetailFetcher(pageConnector);
        var musicDetail = await musicDetailFetcher.FetchAsync(music);

        musicDetails.Add(musicDetail);

        Log.Information("{Count} out of {Musics} has been fetched", ++count, musics.Length);
    }

    var musicCsvHandler = new CsvHandler<Music>(musics, Log.Logger);
    musicCsvHandler.Export(Directory.GetCurrentDirectory(), "musics.csv");

    var musicDetailCsvHandler = new CsvHandler<MusicDetail>(musicDetails, Log.Logger);
    musicDetailCsvHandler.Export(Directory.GetCurrentDirectory(), "music_details.csv");

    return musics;
}

async Task<Stage[]> LogStageDetailAsync()
{
    var stagesFetcher = new StagesFetcher(pageConnector);
    var stages = await stagesFetcher.FetchAsync();

    Log.Information("{Stages} of stages have been fetched", stages.Length);

    int count = 0;
    var stageDetails = new List<StageDetail>();
    foreach (var stage in stages)
    {
        var stageDetailFetcher = new StageDetailFetcher(pageConnector);
        var stageDetail = await stageDetailFetcher.FetchAsync(stage);

        stageDetails.Add(stageDetail);

        Log.Information("{Count} out of {Musics} has been fetched", ++count, stages.Length, stageDetail);
    }

    var stageCsvHandler = new CsvHandler<Stage>(stages, Log.Logger);
    stageCsvHandler.Export(Directory.GetCurrentDirectory(), "stages.csv");

    var stageDetailCsvHandler = new CsvHandler<StageDetail>(stageDetails, Log.Logger);
    stageDetailCsvHandler.Export(Directory.GetCurrentDirectory(), "stage_details.csv");

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