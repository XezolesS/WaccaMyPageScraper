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
    internal sealed class StageFetcherAction : FetcherAction<Stage>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/stageup/detail";

        public StageFetcherAction(Fetcher fetcher) : base(fetcher) { }

        /// <summary>
        /// Fetch player's stage record.
        /// </summary>
        /// <param name="args"><see cref="StageMetadata"/> is needed.</param>
        /// <returns>Fetched player's stage record of given <see cref="StageMetadata"/> in <see cref="Stage"/>.</returns>
        public override async Task<Stage?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
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
            Stage? result = null;

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

                var stageDetailNode = document.DocumentNode.SelectSingleNode("//section[@class='stageup']/div[@class='stageup__detail']");
                var scoreNodes = stageDetailNode.SelectNodes("./ul/li/div/div[@class='stageup__detail__song-info__score']");
                var totalScoreNode = stageDetailNode.SelectSingleNode("./div/div/div[@class='stageup__detail__top__score']");

                // Fetch stage score data
                this._fetcher.Logger?.Information(Localization.Fetcher.Fetching,
                    Localization.Data.Stage + $"({stageArg.Id})");
                progressText.Report(string.Format(Localization.Fetcher.Fetching,
                    Localization.Data.Stage + $"({stageArg.Id})"));

                var scores = new int[3];
                for (int i = 0; i < 3; i++)
                    scores[i] = int.Parse(numericRegex.Match(scoreNodes[i].InnerText).Value);

                var totalScore = int.Parse(numericRegex.Match(totalScoreNode.InnerText).Value);

                result = new Stage(stageArg, scores, totalScore);
            }
            catch (Exception ex)
            {
                this._fetcher.Logger?.Error(ex.Message);

                return null;
            }

            this._fetcher.Logger?.Information(Localization.Fetcher.DataFetched2,
                Localization.Data.Stage,
                result);
            progressText.Report(string.Format(Localization.Fetcher.DataFetched1,
                Localization.Data.Stage));

            return result;
        }
    }
}
