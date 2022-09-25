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
    internal sealed class TotalScoreRankingsFetcherAction : FetcherAction<TotalScoreRankings>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/ranking/totalHighScore/detail";

        public TotalScoreRankingsFetcherAction(Fetcher fetcher) : base(fetcher) { }

        /// <summary>
        /// Fetch a rankings of each total score.
        /// </summary>
        /// <param name="progressText"></param>
        /// <param name="progressPercent"></param>
        /// <param name="args">None.</param>
        /// <returns>A rankings of each total score.</returns>
        public override async Task<TotalScoreRankings?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
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

            var numericRegex = new Regex("[0-9,+]+");

            var rankings = new int[4];
            for (int i = 1; i <= 4; i++)
            {
                this._fetcher.Logger?.Information(Localization.Fetcher.Connecting, Url);

                var parameters = new Dictionary<string, string> { { "rankCategory", i.ToString() } };
                var encodedContent = new FormUrlEncodedContent(parameters);

                var response = await this._fetcher.Client.PostAsync(this.Url, encodedContent).ConfigureAwait(false);

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

                    var rankingNode = document.DocumentNode.SelectSingleNode("//div[@class='ranking__score__rank top-rank']");
                    var rankingImageNode = rankingNode.SelectSingleNode("(./img)[last()]");

                    // Fetch ranking
                    this._fetcher.Logger?.Information(Localization.Fetcher.Fetching,
                        Localization.Data.TotalScoreRanking + $"({i})");
                    progressText.Report(string.Format(Localization.Fetcher.Fetching,
                        Localization.Data.TotalScoreRanking + $"({(Difficulty)(i - 1)})"));

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

                    rankings[i - 1] = ranking;
                }
                catch (Exception ex)
                {
                    this._fetcher.Logger?.Error(ex.Message);

                    return null;
                }
            }

            var result = new TotalScoreRankings(rankings);

            this._fetcher.Logger?.Information(Localization.Fetcher.DataFetched2,
                Localization.Data.TotalScoreRanking,
                result);
            progressText.Report(string.Format(Localization.Fetcher.DataFetched1,
                Localization.Data.TotalScoreRanking));

            return result;
        }
    }
}
