using HtmlAgilityPack;
using Serilog;
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
    public class PlayerFetcher : IFetcher<Player>
    {
        private readonly static string Url = "https://wacca.marv-games.jp/web/player";

        private readonly PageConnector pageConnector;

        public PlayerFetcher(PageConnector pageConnector)
        {
            this.pageConnector = pageConnector;
        }

        public async Task<Player> FetchAsync(params object?[] args)
        {
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            var response = await pageConnector.GetStringAsync(Url);
            Player result = null;

            var numericRegex = new Regex("[0-9]+");

            if (string.IsNullOrEmpty(response))
            {
                this.pageConnector.Logger?.Error("Error occured while connecting to the page!");

                return null;
            }

            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(response);

                var playerDataNode = document.DocumentNode.SelectSingleNode("//div[@class='playdata__playerdata']");

                // Fetch player's name and stuffs
                var userDetailNode = playerDataNode.SelectSingleNode("./div[@class='user-info']/div[@class='user-info__detail']");

                var userNameNode = userDetailNode.SelectSingleNode(".//div[@class='user-info__detail__name']");
                var userLevelNode = userDetailNode.SelectSingleNode(".//div[@class='user-info__detail__lv']/span");
                var userRatingNode = userDetailNode.SelectSingleNode(".//div[@class='rating__wrap']/div/div");

                this.pageConnector.Logger?.Debug("{UserNameNode}, {UserLevelNode}, {UserRatingNode}",
                    userNameNode.InnerText,
                    userLevelNode.InnerText,
                    userRatingNode.InnerText);

                string name = userNameNode.InnerText;
                int level = int.Parse(numericRegex.Match(userLevelNode.InnerText).Value);
                int rate = int.Parse(userRatingNode.InnerText);

                // Fetch a stage icon file name to get the player's stage info
                var userIconNode = playerDataNode.SelectSingleNode("./div[@class='user-info']/div[@class='user-info__icon']");

                var userIconImgNode = userIconNode.SelectSingleNode("./div[@class='user-info__icon__stage']/img");

                int stageCleared = 0;
                StageGrade stageGrade = 0;
                if (userIconImgNode != null)
                {
                    var stageIconImgSrc = userIconImgNode.Attributes["src"].Value;

                    this.pageConnector.Logger?.Debug("{UserIconImgNode}", stageIconImgSrc);

                    var stageRegex = new Regex(@"^\/img\/web\/stage\/rank\/stage_icon_[0-9]+_[1-3].png$");
                    if (stageRegex.IsMatch(stageIconImgSrc))
                    {
                        var stageIconNumbers = new Regex("[0-9]+_[1-3]").Match(stageIconImgSrc).Value.Split('_');

                        stageCleared = int.Parse(stageIconNumbers[0]);
                        stageGrade = (StageGrade)int.Parse(stageIconNumbers[1]);
                    }
                }

                // Fetch player's play counts
                var playCountNode = playerDataNode.SelectSingleNode("./div[@class='play-count']");

                var playCountDetailNodes = playCountNode.SelectNodes(".//dd"); // 0: Total, 1: Versus Mode, 2: Co-op mode

                this.pageConnector.Logger?.Debug("{PlayCountDetailNodes_0}, {PlayCountDetailNodes_1}, {PlayCountDetailNodes_2}",
                    playCountDetailNodes[0].InnerText, playCountDetailNodes[1].InnerText, playCountDetailNodes[2].InnerText);

                int playCount = int.Parse(numericRegex.Match(playCountDetailNodes[0].InnerText).Value);
                int playCountVersus = int.Parse(numericRegex.Match(playCountDetailNodes[1].InnerText).Value);
                int playCountCoop = int.Parse(numericRegex.Match(playCountDetailNodes[2].InnerText).Value);

                // Fetch player's RP earned and spent
                var pointNode = playerDataNode.SelectSingleNode("./div[@class='poss-wp']");

                var pointDetailNodes = pointNode.SelectNodes(".//dd"); // 0: Currently has (not used), 1: Total earend, 2: Total spent

                this.pageConnector.Logger?.Debug("{PointDetailNode_1}, {PointDetailNode_2}",
                    pointDetailNodes[1].InnerText, pointDetailNodes[2].InnerText);

                int totalRpEarned = int.Parse(numericRegex.Match(pointDetailNodes[1].InnerText).Value);
                int totalRpSpent = int.Parse(numericRegex.Match(pointDetailNodes[2].InnerText).Value);

                result = new Player
                {
                    Name = name,
                    Level = level,
                    Rate = rate,
                    StageCleared = stageCleared,
                    StageGrade = stageGrade,
                    PlayCount = playCount,
                    PlayCountVersus = playCountVersus,
                    PlayCountCoop = playCountCoop,
                    TotalRpEarned = totalRpEarned,
                    TotalRpSpent = totalRpSpent,
                };
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
