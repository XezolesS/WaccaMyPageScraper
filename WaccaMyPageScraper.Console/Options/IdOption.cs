using Serilog;

namespace WaccaMyPageScraper.Console.Options
{
    internal class IdOption : Option
    {
        private int loginTries = 0;

        public IdOption(ILogger? logger, Fetcher fetcher, string parameter) 
            : base(logger, fetcher, parameter) { }

        public override object Execute()
        {
            Fetcher fetcher;

            int loginTries = 0;
            while (true)
            {
                if (loginTries >= 10)
                {
                    logger?.Error("Can not login to WACCA my page. Check your internet connection");

                    return null;
                }

                fetcher = new Fetcher(this.parameter, Log.Logger);
                var loginResult = Task.Run(async () => await fetcher.LoginAsync()).Result;
                
                if (loginResult)
                {
                    Log.Information("Login succeed!");

                    break;
                }

                Log.Error("Login failed!");
                loginTries++;

                Task.Delay(1000);
            }
            
            return fetcher;
        }
    }
}
