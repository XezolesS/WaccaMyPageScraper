// See https://aka.ms/new-console-template for more information
using WaccaMyPageScraper;
using WaccaMyPageScraper.Data;
using Microsoft.Extensions.Logging;
using Serilog;

// Create logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

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

Log.CloseAndFlush();

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
    using (var pageScraper = new PageConnector(aimeId, Log.Logger))
        result = await pageScraper.LoginAsync();
    
    return result;
}
#endregion