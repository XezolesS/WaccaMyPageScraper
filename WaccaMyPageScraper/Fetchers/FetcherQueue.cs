using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Fetchers
{
    public class FetcherQueue<T>
    {
        private readonly PageConnector pageConnector;

        private List<IFetcher<T>> fetchers = new List<IFetcher<T>>();

        public FetcherQueue(PageConnector pageConnector)
        {
            this.pageConnector = pageConnector;
        }

        public void Add(IFetcher<T> fetcher)
        {
            this.fetchers.Add(fetcher);
        }

        public void Add(IEnumerable<IFetcher<T>> fetchers)
        {
            this.fetchers.AddRange(fetchers);
        }

        public async Task<T> FetchNextAsync()
        {
            var next = this.fetchers.First();
            var result = await next.FetchAsync();

            this.fetchers.RemoveAt(0);

            return result;
        }

        public async Task<T[]> FetchAllAsync()
        {
            var result = new List<T>();
            while (!this.IsEmpty())
            {
                result.Add(await this.FetchNextAsync());
            }

            return result.ToArray();
        }

        public bool IsEmpty() => this.fetchers.Count == 0;
    }
}
