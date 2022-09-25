using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.Resources;

namespace WaccaMyPageScraper.FetcherActions
{
    internal sealed class PlayerFetcherAction : FetcherAction<Player>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/player";
    
        public PlayerFetcherAction(Fetcher fetcher) : base(fetcher) { }

        /// <summary>
        /// Fetch player's data.
        /// </summary>
        /// <param name="args">No argument needed.</param>
        /// <returns>Fetched player data in <see cref="Player"/>, null if it's failed.</returns>
        public override async Task<Player?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
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

            this._fetcher.Logger?.Information(Localization.Fetcher.Connecting, Url);

            var response = await this._fetcher.Client.GetStringAsync(this.Url).ConfigureAwait(false);
            Player? result = null;

            if (string.IsNullOrEmpty(response))
            {
                this._fetcher.Logger?.Error(Localization.Fetcher.ConnectionError);

                return null;
            }

            this._fetcher.Logger?.Information(Localization.Fetcher.Connected);

            try
            {
                var numericRegex = new Regex("[0-9]+");

                var document = new HtmlDocument();
                if (!this.TryLoadHtml(ref document, response))
                {
                    this._fetcher.LoginStatus = LoginStatus.LoggedOff;
                    return null;
                }

                var playerDataNode = document.DocumentNode.SelectSingleNode("//div[@class='playdata__playerdata']");

                // Fetch player's name and stuffs
                this._fetcher.Logger?.Information(Localization.Fetcher.Fetching,
                    Localization.Data.Player);

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

                StageMetadata stage = new StageMetadata();
                if (userIconImgNode is not null)
                {
                    var stageIconImgSrc = userIconImgNode.Attributes["src"].Value;

                    this._fetcher.Logger?.Debug("Icon Image Source: {UserIconImgNode}", stageIconImgSrc);

                    var stageIconFileName = new Regex(@"stage_icon_[0-9]+_[1-3].png").Match(stageIconImgSrc).Value;
                    var stageIconNumbers = new Regex("[0-9]+_[1-3]").Match(stageIconFileName).Value.Split('_');

                    int id = int.Parse(stageIconNumbers[0]);
                    StageGrade grade = (StageGrade)int.Parse(stageIconNumbers[1]);

                    stage = new StageMetadata(id, grade);
                }

                // Fetch player's play counts
                var playCountNode = playerDataNode.SelectSingleNode("./div[@class='play-count']");

                var playCountDetailNodes = playCountNode.SelectNodes(".//dd"); // 0: Total, 1: Versus Mode, 2: Co-op mode

                this._fetcher.Logger?.Debug("{PlayCountDetailNodes_0}, {PlayCountDetailNodes_1}, {PlayCountDetailNodes_2}",
                    playCountDetailNodes[0].InnerText, playCountDetailNodes[1].InnerText, playCountDetailNodes[2].InnerText);

                int playCount = int.Parse(numericRegex.Match(playCountDetailNodes[0].InnerText).Value);
                int playCountVersus = int.Parse(numericRegex.Match(playCountDetailNodes[1].InnerText).Value);
                int playCountCoop = int.Parse(numericRegex.Match(playCountDetailNodes[2].InnerText).Value);

                // Fetch player's RP earned and spent
                var pointNode = playerDataNode.SelectSingleNode("./div[@class='poss-wp']");

                var pointDetailNodes = pointNode.SelectNodes(".//dd"); // 0: Currently has (not used), 1: Total earend, 2: Total spent

                this._fetcher.Logger?.Debug("{PointDetailNode_1}, {PointDetailNode_2}",
                    pointDetailNodes[1].InnerText, pointDetailNodes[2].InnerText);

                int totalRpEarned = int.Parse(numericRegex.Match(pointDetailNodes[1].InnerText).Value);
                int totalRpSpent = int.Parse(numericRegex.Match(pointDetailNodes[2].InnerText).Value);

                result = new Player(name, level, rate, stage, playCount, playCountVersus, playCountCoop, totalRpEarned, totalRpSpent);
            }
            catch (Exception ex)
            {
                this._fetcher.Logger?.Error(ex.Message);

                return null;
            }

            this._fetcher.Logger?.Information(Localization.Fetcher.DataFetched2,
                Localization.Data.Player, result);

            return result;
        }
    }
}
