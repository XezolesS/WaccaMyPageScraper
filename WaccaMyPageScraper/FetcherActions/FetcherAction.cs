using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.FetcherActions
{
    /// <summary>
    /// Abstraction class for fetcher actions.<br/>
    /// </summary>
    /// <typeparam name="T">Return type of the fetcher.</typeparam>
    internal abstract class FetcherAction<T>
    {
        protected Fetcher _fetcher = null;

        protected Uri BaseUrl => new Uri("https://wacca.marv-games.jp/");

        protected abstract string Url { get; }

        public FetcherAction(Fetcher _fetcher)
        {
            this._fetcher = _fetcher;
        }
        
        public abstract Task<T?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args);

        protected int CalculatePercent(int count, int total) => (int)(((double)count / total) * 100);
    }
}
