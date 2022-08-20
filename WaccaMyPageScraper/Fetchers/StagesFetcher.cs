using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Fetchers
{
    public class StagesFetcher : IFetcher<Stage[]>
    {
        private readonly static string Url = "https://wacca.marv-games.jp/web/stageup";

        private readonly PageConnector pageConnector;

        public StagesFetcher(PageConnector pageConnector)
        {
            this.pageConnector = pageConnector;
        }

        public async Task<Stage[]> FetchAsync(params object?[] args)
        {
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            var response = await pageConnector.GetStringAsync(Url);
            List<Stage> result = new List<Stage>();

            if (string.IsNullOrEmpty(response))
            {
                this.pageConnector.Logger?.Error("Error occured while connecting to the page!");

                return null;
            }

            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(response);

                var stageListNode = document.DocumentNode.SelectSingleNode("//section[@class='stageup']/ul[@class='stageup__list']");

                var stageItemNodes = stageListNode.SelectNodes("./form/a[@class='stageup__list__link']");
                foreach (var node in stageItemNodes)
                {
                    var stageNameNode = node.SelectSingleNode("./li/div/div[@class='stageup__list__top__name']");
                    var stageIconNode = node.SelectSingleNode("./li/div[@class='stageup__list__course-icon']/img");

                    Stage stage;
                    if (stageIconNode is null)
                    {
                        int id = int.Parse(node.Attributes["value"].Value);
                        var name = stageNameNode.InnerText;
                        stage = new Stage(id, name, StageGrade.NotCleared);
                    }
                    else
                    {
                        var stageIconImgSrc = stageIconNode.Attributes["src"].Value;

                        var converter = TypeDescriptor.GetConverter(typeof(Stage));
                        stage = (Stage)converter.ConvertFrom(stageIconImgSrc);
                    }

                    this.pageConnector.Logger?.Debug("Fetched stage data: {Stage}", stage);

                    result.Add(stage);
                }
            }
            catch (Exception ex)
            {
                this.pageConnector.Logger?.Error(ex.Message);

                return null;
            }

            return result.ToArray();
        }
    }
}
