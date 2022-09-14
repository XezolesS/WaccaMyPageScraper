using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Fetchers
{
    public abstract class Fetcher<T>
    {
        protected Uri BaseUrl => new Uri("https://wacca.marv-games.jp/");

        protected abstract string Url { get; }

        protected PageConnector pageConnector { get; set; }

        public Fetcher(PageConnector pageConnector)
        {
            this.pageConnector = pageConnector;
        }

        public async Task<T?> FetchAsync(params object?[] args) => await FetchAsync(new Progress<string>(), new Progress<int>(), args);
        public abstract Task<T?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args);

        

        protected int CalculatePercent(int count, int total) => (int)(((double)count / total) * 100);
    }
}
