using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.FetcherActions
{
    internal sealed class StageRankingFetcherAction : FetcherAction<StageRanking>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/ranking/stageupScore/detail";

        public StageRankingFetcherAction(Fetcher fetcher) : base(fetcher) { }

        /// <summary>
        /// Fetch player's stage ranking.
        /// </summary>
        /// <param name="args"><see cref="StageMetadata"/> is needed.</param>
        /// <returns>Fetched player's stage record of given <see cref="StageMetadata"/> in <see cref="Stage"/>.</returns>
        public override async Task<StageRanking?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
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

            if (args[0] is null || args[0] is not StageMetadata)
            {
                this._fetcher.Logger?.Error(Localization.Fetcher.NoArgument,
                    MethodBase.GetCurrentMethod().Name,
                    typeof(StageMetadata));

                return null;
            }

            this._fetcher.Logger?.Information(Localization.Fetcher.Connecting, Url);

            var stageArg = args[0] as StageMetadata;

            var parameters = new Dictionary<string, string> { { "trialClass", stageArg.Id.ToString() } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await this._fetcher.Client.PostAsync(this.Url, encodedContent).ConfigureAwait(false);
            StageRanking? result = null;

            if (!response.IsSuccessStatusCode)
            {
                this._fetcher.Logger?.Error(Localization.Fetcher.ConnectionError);

                return null;
            }

            this._fetcher.Logger?.Information(Localization.Fetcher.Connected);

            try
            {
                var numericRegex = new Regex("[0-9]+");

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
                    Localization.Data.StageRanking + $"({stageArg.Id})");
                progressText.Report(string.Format(Localization.Fetcher.Fetching,
                        Localization.Data.StageRanking + $"({stageArg.Id})"));

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

                result = new StageRanking(stageArg, ranking);
            }
            catch (Exception ex)
            {
                this._fetcher.Logger?.Error(ex.Message);

                return null;
            }

            this._fetcher.Logger?.Information(Localization.Fetcher.DataFetched2,
                Localization.Data.StageRanking,
                result);
            progressText.Report(string.Format(Localization.Fetcher.DataFetched1,
                Localization.Data.StageRanking));

            return result;
        }
    }
}
