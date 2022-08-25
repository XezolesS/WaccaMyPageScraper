using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Console.Options
{
    internal class OutputOption : Option
    {
        public OutputOption(ILogger? logger, PageConnector pageConnector, string parameter) 
            : base(logger, pageConnector, parameter) { }

        public override object Execute()
        {
            var result = this.parameter;

            try
            {
                Path.GetFullPath(this.parameter);

                if (string.IsNullOrEmpty(Path.GetExtension(this.parameter)))
                    result += ".csv";

                this.logger?.Debug("Set output path: {Path}", this.parameter);
            }
            catch
            {
                this.logger?.Debug("No output path set.");

                return null;
            }

            return result;
        }
    }
}
