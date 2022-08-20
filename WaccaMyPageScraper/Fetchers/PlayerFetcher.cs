using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// Fetch player's data.
        /// </summary>
        /// <param name="args">No argument needed.</param>
        /// <returns>Fetched player data in <see cref="Player"/>, null if it's failed.</returns>
        public async Task<Player?> FetchAsync(params object?[] args)
        {
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            this.pageConnector.Logger?.Information("Trying to connect to {URL}", Url);

            var response = await pageConnector.GetStringAsync(Url);
            Player? result = null;

            if (string.IsNullOrEmpty(response))
            {
                this.pageConnector.Logger?.Error("Error occured while connecting to the page!");

                return null;
            }

            this.pageConnector.Logger?.Information("Connection successful");

            try
            {
                var numericRegex = new Regex("[0-9]+");

                var document = new HtmlDocument();
                document.LoadHtml(response);

                var playerDataNode = document.DocumentNode.SelectSingleNode("//div[@class='playdata__playerdata']");

                // Fetch player's name and stuffs
                var userDetailNode = playerDataNode.SelectSingleNode("./div[@class='user-info']/div[@class='user-info__detail']");

                var userNameNode = userDetailNode.SelectSingleNode(".//div[@class='user-info__detail__name']");
                var userLevelNode = userDetailNode.SelectSingleNode(".//div[@class='user-info__detail__lv']/span");
                var userRatingNode = userDetailNode.SelectSingleNode(".//div[@class='rating__wrap']/div/div");

                string name = userNameNode.InnerText;
                int level = int.Parse(numericRegex.Match(userLevelNode.InnerText).Value);
                int rate = int.Parse(userRatingNode.InnerText);

                // Fetch a stage icon file name to get the player's stage info
                var userIconNode = playerDataNode.SelectSingleNode("./div[@class='user-info']/div[@class='user-info__icon']");

                var userIconImgNode = userIconNode.SelectSingleNode("./div[@class='user-info__icon__stage']/img");

                Stage stage = new Stage();
                if (userIconImgNode is not null)
                {
                    var stageIconImgSrc = userIconImgNode.Attributes["src"].Value;

                    this.pageConnector.Logger?.Debug("Icon Image Source: {UserIconImgNode}", stageIconImgSrc);

                    var converter = TypeDescriptor.GetConverter(typeof(Stage));
                    stage = (Stage)converter.ConvertFrom(stageIconImgSrc);
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

                result = new Player(name, level, rate, stage, playCount, playCountVersus, playCountCoop, totalRpEarned, totalRpSpent);
            }
            catch (Exception ex)
            {
                this.pageConnector.Logger?.Error(ex.Message);

                return null;
            }

            this.pageConnector.Logger?.Information("Successfully fetched player data: {Result}", result);

            return result;
        }
    }
}
