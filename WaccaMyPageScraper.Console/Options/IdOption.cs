using Serilog;

namespace WaccaMyPageScraper.Console.Options
{
    internal class IdOption : Option
    {
        private int loginTries = 0;

        public IdOption(ILogger? logger, PageConnector pageConnector, string parameter) 
            : base(logger, pageConnector, parameter) { }

        public override object Execute()
        {
            PageConnector connector;

            int loginTries = 0;
            while (true)
            {
                if (loginTries >= 10)
                {
                    logger?.Error("Can not login to WACCA my page. Check your internet connection");

                    return null;
                }

                connector = new PageConnector(parameter, Log.Logger);
                var loginResult = Task.Run(async () => await connector.LoginAsync()).Result;
                
                if (loginResult)
                {
                    Log.Information("Login succeed!");

                    break;
                }

                Log.Error("Login failed!");
                loginTries++;

                Task.Delay(1000);
            }
            
            return connector;
        }
    }
}
