// See https://aka.ms/new-console-template for more information
using WaccaMyPageScraper;
using WaccaMyPageScraper.Data;

// Retreive Aime Id
var aimeId = GetAimeId();
var isLoginSuccess = false;
User userData = null;

using (var pageScraper = new PageConnector(aimeId))
{
    isLoginSuccess = await pageScraper.LoginAsync();
    userData = await pageScraper.FetchUserAsync();
}

if (!isLoginSuccess)
{
    Console.WriteLine("Login Failed");
    return;
}

Console.WriteLine("Login Succeed");
Console.WriteLine(userData);

string GetAimeId()
{
    string aimeId;
    while (true)
    {
        Console.Write("Type your Aime ID >> ");
        aimeId = Console.ReadLine();

        if (string.IsNullOrEmpty(aimeId))
            Console.WriteLine("The given ID is not valid!");
        else 
            return aimeId;
    }
}