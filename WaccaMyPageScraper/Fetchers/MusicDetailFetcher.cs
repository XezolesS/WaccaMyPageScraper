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
    public class MusicDetailFetcher : IFetcher<MusicDetail>
    {
        private readonly static string Url = "https://wacca.marv-games.jp/web/music/detail";

        private readonly PageConnector pageConnector;

        public MusicDetailFetcher(PageConnector pageConnector)
        {
            this.pageConnector = pageConnector;
        }

        public async Task<MusicDetail> FetchAsync(params object?[] args)
        {
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            if (args[0] == null)
            {
                this.pageConnector.Logger?.Error("MusicDetailFetcher.FetchAsync() must have at least 1 argument!");

                return null;
            }

            var parameters = new Dictionary<string, string> { { "musicId", args[0].ToString() } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await this.pageConnector.PostAsync(Url, encodedContent);
            MusicDetail result = null;

            var numericRegex = new Regex("[0-9,+]+");

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

                var scoreDetailNode = document.DocumentNode.SelectSingleNode("//section[@class='playdata']/div/div[@class='playdata__score-detail']");
                var titleNode = scoreDetailNode.SelectSingleNode(".//div[@class='song-info__name']");
                var artistNode = scoreDetailNode.SelectSingleNode(".//div[@class='song-info__artist']");

                int id = int.Parse(args[0].ToString());
                string title = titleNode.InnerText;
                string artist = artistNode.InnerText;

                // Fetch music data
                List<string> levels = new List<string>();
                List<int> playCounts = new List<int>();
                List<int> scores = new List<int>();
                List<Achieve> achieves = new List<Achieve>();
                var detailListsNodes = scoreDetailNode.SelectNodes("./ul[@class='score-detail__list']/li");
                foreach (var node in detailListsNodes)
                {
                    var songTopNode = node.SelectSingleNode("./div/div[@class='song-info__top']");
                    var songBottomNode = node.SelectSingleNode("./div/div[@class='song-info__bottom']");

                    var levelNode = songTopNode.SelectSingleNode("./div[@class='song-info__top__lv']/div");
                    var playCountNode = songTopNode.SelectSingleNode("./div[@class='song-info__top__play-count']");
                    var scoreNode = songBottomNode.SelectSingleNode("./div[@class='song-info__score']");
                    var achieveNode = songBottomNode.SelectSingleNode("./div[@class='score-detail__icon']/div/img[@alt='achieveimage']");

                    var level = new Regex("[0-9+]+").Match(levelNode.InnerText).Value;
                    var playCount = int.Parse(numericRegex.Match(playCountNode.InnerText).Value);
                    var score = int.Parse(numericRegex.Match(scoreNode.InnerText).Value);
                    var achieve = achieveNode.Attributes["src"].Value.Split('/').Last() switch
                    {
                        "achieve1.png" => Achieve.Clear,
                        "achieve2.png" => Achieve.Missless,
                        "achieve3.png" => Achieve.FullCombo,
                        "achieve4.png" => Achieve.AllMarvelous,
                        _ => Achieve.NoAchieve
                    };

                    levels.Add(level);
                    playCounts.Add(playCount);
                    scores.Add(score);
                    achieves.Add(achieve);
                }

                result = new MusicDetail
                {
                    Id = id,
                    Title = title,
                    Genre = (Genre)args[1],
                    Levels = levels.ToArray(),
                    Artist = artist,
                    PlayCounts = playCounts.ToArray(),
                    Scores = scores.ToArray(),
                    Achieves = achieves.ToArray(),
                };            }
            catch (Exception ex)
            {
                this.pageConnector.Logger?.Error(ex.Message);

                return null;
            }

            return result;
        }
    }
}
