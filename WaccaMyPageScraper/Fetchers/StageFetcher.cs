using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Fetchers
{
    public sealed class StageFetcher : Fetcher<Stage>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/stageup/detail";

        public StageFetcher(PageConnector pageConnector) : base(pageConnector) { }

        /// <summary>
        /// Fetch player's stage record.
        /// </summary>
        /// <param name="args"><see cref="StageMetadata"/> is needed.</param>
        /// <returns>Fetched player's stage record of given <see cref="StageMetadata"/> in <see cref="Stage"/>.</returns>
        public override async Task<Stage?> FetchAsync(params object?[] args)
        {
            // Connect to the page and get an HTML document.
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            if (args[0] is null || args[0] is not StageMetadata)
            {
                this.pageConnector.Logger?.Error("StageFetcher.FetchAsync() must have a Stage class argument!");

                return null;
            }

            var stageArg = args[0] as StageMetadata;
            if (stageArg.Grade == StageGrade.NotCleared)
            {
                this.pageConnector.Logger?.Information("There is no record of {StageName}.", stageArg.Name);

                return new Stage(stageArg, new int[3] { 0, 0, 0 });
            }

            this.pageConnector.Logger?.Information("Trying to connect to {URL}", Url);

            var parameters = new Dictionary<string, string> { { "trialClass", stageArg.Id.ToString() } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await this.pageConnector.Client.PostAsync(this.Url, encodedContent).ConfigureAwait(false);
            Stage? result = null;

            if (!response.IsSuccessStatusCode)
            {
                this.pageConnector.Logger?.Error("Error occured while connecting to the page!");

                return null;
            }

            this.pageConnector.Logger?.Information("Connection successful");

            try
            {
                var numericRegex = new Regex("[0-9]+");

                var responseContent = await response.Content.ReadAsStringAsync();

                // Check response content HTML to find out if it's an error page.
                var document = new HtmlDocument();
                document.LoadHtml(responseContent);

                var stageDetailNode = document.DocumentNode.SelectSingleNode("//section[@class='stageup']/div[@class='stageup__detail']");
                var scoreNodes = stageDetailNode.SelectNodes("./ul/li/div/div[@class='stageup__detail__song-info__score']");

                var scores = new int[3];
                for (int i = 0; i < 3; i++)
                    scores[i] = int.Parse(numericRegex.Match(scoreNodes[i].InnerText).Value);
               
                result = new Stage(stageArg, scores);
            }
            catch (Exception ex)
            {
                this.pageConnector.Logger?.Error(ex.Message);

                return null;
            }

            this.pageConnector.Logger?.Information("Successfully fetched stage data: {Result}", result);

            return result;
        }
    }
}
