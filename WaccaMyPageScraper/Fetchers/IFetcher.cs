using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Fetchers
{
    public interface IFetcher<T>
    {
        public PageConnector Connector { get; set; }

        public Task<T> FetchAsync();
    }
}
