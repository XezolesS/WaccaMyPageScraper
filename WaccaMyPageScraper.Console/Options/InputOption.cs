using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Console.Options
{
    internal class InputOption : Option
    {
        public InputOption(ILogger? logger, PageConnector pageConnector, string parameter)
            : base(logger, pageConnector, parameter) { }

        public override object Execute()
        {
            var result = this.parameter;

            try
            {
                Path.GetFullPath(this.parameter);

                this.logger?.Debug("Set input path: {Path}", this.parameter);
            }
            catch
            {
                this.logger?.Debug("No input path set.");

                return null;
            }

            return result;
        }
    }
}
