using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;

namespace WaccaMyPageScraper.Fetchers
{
    public class PlayerFetcher : IFetcher<Player>
    {
        private readonly static string Url = "https://wacca.marv-games.jp/web/player";

        private readonly PageConnector pageConnector;

        public PlayerFetcher(PageConnector pageConnector)
        {
            this.pageConnector = pageConnector;
        }

        public Task<Player> FetchAsync()
        {
            throw new NotImplementedException();
        }

        /*
         // Get a response from "My Page Top".
            var response = await this.CallMyPage();

            var document = new HtmlDocument();
            document.LoadHtml(response);

            // Select user data from HTML and convert to the data.
            var userInfoNodes = document.DocumentNode.SelectSingleNode("//section[@class='user-info']");

            var userIconNode = userInfoNodes.SelectSingleNode("./div[@class='user-info__icon']");
            var userDetailNode = userInfoNodes.SelectSingleNode("./div[@class='user-info__detail']");

            var stageIconSrc = userIconNode.SelectSingleNode("./div[@class='user-info__icon__stage']/img").Attributes["src"].Value;
            var stageIconNumbers = stageIconSrc.Split('/').Last()
                .Replace("stage_icon_", "")
                .Replace(".png", "") 
                .Split('_');

            var name = userDetailNode.SelectSingleNode("./div/div[@class='user-info__detail__name']").InnerText;
            var level = userDetailNode.SelectSingleNode("./div/div[@class='user-info__detail__lv']/span").InnerText;
            var rating = userDetailNode.SelectSingleNode("./div[@class='rating__wrap']/div/div").InnerText;

            return new Player
            {
                Name = name,
                Level = int.Parse(level.Replace("Lv.", "")),
                Rate = int.Parse(rating),
                StageCleared = int.Parse(stageIconNumbers[0]),
                StageGrade = int.Parse(stageIconNumbers[1])
            };
        */
    }
}
