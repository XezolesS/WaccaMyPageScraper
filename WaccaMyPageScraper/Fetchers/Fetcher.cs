using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Fetchers
{
    public abstract class Fetcher<T>
    {
        protected abstract string Url { get; }

        protected PageConnector pageConnector { get; set; }

        public Fetcher(PageConnector pageConnector)
        {
            this.pageConnector = pageConnector;
        }

        public abstract Task<T?> FetchAsync(params object?[] args);
    }
}
