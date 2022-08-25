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
        internal readonly PageConnector pageConnector;
        internal readonly string parameter;

        public Option(ILogger? logger, PageConnector pageConnector, string parameter)
        {
            this.logger = logger;
            this.pageConnector = pageConnector;
            this.parameter = parameter;
        }

        public abstract object Execute();
    }
}
