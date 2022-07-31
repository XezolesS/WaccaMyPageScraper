// See https://aka.ms/new-console-template for more information
using WaccaMyPageScraper;

// Retreive Aime Id
var aimeId = GetAimeId();
var pageScraper = new WaccaMyPage(aimeId);
var login = await pageScraper.LoginAsync();
var fetchUserResponse = await pageScraper.FetchUserAsync();

Console.WriteLine();

Console.WriteLine(login ? "Login Success" : "Login Failed");
Console.WriteLine(fetchUserResponse);

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