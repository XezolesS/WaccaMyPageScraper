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
    public class StageDetailFetcher : IFetcher<StageDetail>
    {
        private readonly static string Url = "https://wacca.marv-games.jp/web/stageup/detail";

        private readonly PageConnector pageConnector;

        public StageDetailFetcher(PageConnector pageConnector)
        {
            this.pageConnector = pageConnector;
        }

        public async Task<StageDetail> FetchAsync(params object?[] args)
        {
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            if (args[0] is null || args[0] is not Stage)
            {
                this.pageConnector.Logger?.Error("MusicDetailFetcher.FetchAsync() must have a Stage class argument!");

                return null;
            }

            var stageArg = args[0] as Stage;
            if (stageArg.Grade == StageGrade.NotCleared)
            {
                return new StageDetail(stageArg, new int[3] { 0, 0, 0 });
            }

            var parameters = new Dictionary<string, string> { { "trialClass", stageArg.Id.ToString() } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await this.pageConnector.PostAsync(Url, encodedContent);
            StageDetail result = null;

            if (!response.IsSuccessStatusCode)
            {
                this.pageConnector.Logger?.Error("Error occured while connecting to the page!");

                return null;
            }

            try
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Check response content HTML to find out if it's an error page.
                var document = new HtmlDocument();
                document.LoadHtml(responseContent);

                var stageDetailNode = document.DocumentNode.SelectSingleNode("//section[@class='stageup']/div[@class='stageup__detail']");
                var scoreNodes = stageDetailNode.SelectNodes("./ul/li/div/div[@class='stageup__detail__song-info__score']");

                var numericRegex = new Regex("[0-9]+");

                var scores = new int[3];
                for (int i = 0; i < 3; i++)
                    scores[i] = int.Parse(numericRegex.Match(scoreNodes[i].InnerText).Value);
               
                result = new StageDetail(stageArg, scores);
            }
            catch (Exception ex)
            {
                this.pageConnector.Logger?.Error(ex.Message);

                return null;
            }

            return result;
        }
    }
}
