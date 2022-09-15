using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Console.Options
{
    internal abstract class Option
    {
        internal readonly ILogger? logger;
        internal readonly Fetcher fetcher;
        internal readonly string parameter;

        public Option(ILogger? logger, Fetcher fetcher, string parameter)
        {
            this.logger = logger;
            this.fetcher = fetcher;
            this.parameter = parameter;
        }

        public abstract object Execute();
    }
}
