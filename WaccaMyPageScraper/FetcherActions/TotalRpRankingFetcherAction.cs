using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.FetcherActions
{
    internal class TotalRpRankingFetcherAction : FetcherAction<TotalRpRanking>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/ranking/wpScore/detail";

        public TotalRpRankingFetcherAction(Fetcher fetcher) : base(fetcher) { }

        /// <summary>
        /// Fetch a ranking of total RP earned.
        /// </summary>
        /// <param name="progressText"></param>
        /// <param name="progressPercent"></param>
        /// <param name="args"></param>
        /// <returns>A ranking of total RP earned.</returns>
        public override async Task<TotalRpRanking?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
        {
            // Connect to the page and get an HTML document.
            if (!this._fetcher.IsLoggedOn())
            {
                // Logging and Reporting progress.
                this._fetcher.Logger?.Error(Localization.Fetcher.NotLoggedIn);

                progressText.Report(Localization.Fetcher.LoggedOff);
                progressPercent.Report(0);

                return null;
            }

            var result = new TotalRpRanking();
            var numericRegex = new Regex("[0-9,+]+");

            this._fetcher.Logger?.Information(Localization.Fetcher.Connecting, Url);

            var response = await this._fetcher.Client.GetAsync(this.Url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                this._fetcher.Logger?.Error(Localization.Fetcher.ConnectionError);

                return null;
            }

            this._fetcher.Logger?.Information(Localization.Fetcher.Connected);

            try
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Check response content HTML to find out if it's an error page.
                var document = new HtmlDocument();
                if (!this.TryLoadHtml(ref document, responseContent))
                {
                    this._fetcher.LoginStatus = LoginStatus.LoggedOff;
                    return null;
                }

                var rankingNode = document.DocumentNode.SelectSingleNode("//div[@class='ranking__wp-score__rank top-rank']");
                var rankingImageNode = rankingNode.SelectSingleNode("(./img)[last()]");

                // Fetch ranking
                this._fetcher.Logger?.Information(Localization.Fetcher.Fetching,
                    Localization.Data.TotalRpRanking);

                var ranking = -1;
                if (rankingImageNode is null)
                {
                    ranking = numericRegex.IsMatch(rankingNode.InnerText) ?
                        int.Parse(numericRegex.Match(rankingNode.InnerText).Value) :
                        -1;
                }
                else
                {
                    ranking = int.Parse(
                        numericRegex.Match(rankingImageNode.Attributes["src"].Value)
                        .Value);
                }

                result = new TotalRpRanking(ranking);
            }
            catch (Exception ex)
            {
                this._fetcher.Logger?.Error(ex.Message);

                return null;
            }

            this._fetcher.Logger?.Information(Localization.Fetcher.DataFetched2,
                Localization.Data.TotalRpRanking,
                result);

            return result;
        }
    }
}
