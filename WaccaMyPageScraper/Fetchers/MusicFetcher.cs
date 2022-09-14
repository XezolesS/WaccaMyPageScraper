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
using WaccaMyPageScraper.Resources;

namespace WaccaMyPageScraper.Fetchers
{
    public sealed class MusicFetcher : Fetcher<Music>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/music/detail";

        private string ImageUrl => "https://wacca.marv-games.jp/img/music/";

        public MusicFetcher(PageConnector pageConnector) : base(pageConnector) { }

        /// <summary>
        /// Fetch player's play record of the music.
        /// </summary>
        /// <param name="args"><see cref="MusicMetadata"/> is needed.</param>
        /// <returns>Fetched player's record of given <see cref="MusicMetadata"/> in <see cref="Music"/>.</returns>
        public override async Task<Music?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
        {
            // Connect to the page and get an HTML document.
            if (!this.pageConnector.IsLoggedOn())
            {
                // Logging and Reporting progress.
                this.pageConnector.Logger?.Error(Localization.Fetcher.NotLoggedIn);

                progressText.Report(Localization.Connector.LoggedOff);
                progressPercent.Report(0);

                return null;
            }

            if (args[0] is null || args[0] is not MusicMetadata)
            {
                this.pageConnector.Logger?.Error(Localization.Fetcher.NoArgument,
                    MethodBase.GetCurrentMethod().Name, 
                    typeof(MusicMetadata));

                return null;
            }

            this.pageConnector.Logger?.Information(Localization.Fetcher.Connecting, Url);

            var musicArg = args[0] as MusicMetadata;

            var parameters = new Dictionary<string, string> { { "musicId", musicArg.Id.ToString() } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await this.pageConnector.Client.PostAsync(this.Url, encodedContent).ConfigureAwait(false);
            Music? result = null;

            if (!response.IsSuccessStatusCode)
            {
                this.pageConnector.Logger?.Error(Localization.Fetcher.ConnectionError);

                return null;
            }

            this.pageConnector.Logger?.Information(Localization.Fetcher.Connected);

            try
            {
                var numericRegex = new Regex("[0-9,+]+");

                var responseContent = await response.Content.ReadAsStringAsync();

                // Check response content HTML to find out if it's an error page.
                var document = new HtmlDocument();
                document.LoadHtml(responseContent);

                var scoreDetailNode = document.DocumentNode.SelectSingleNode("//section[@class='playdata']/div/div[@class='playdata__score-detail']");
                var artistNode = scoreDetailNode.SelectSingleNode(".//div[@class='song-info__artist']");

                string artist = artistNode.InnerText;

                // Fetch music data
                this.pageConnector.Logger?.Information(Localization.Fetcher.Fetching,
                    Localization.Data.Music + $"({musicArg.Id})");
                
                List<int> playCounts = new List<int>();
                List<int> scores = new List<int>();
                List<Achieve> achieves = new List<Achieve>();

                var detailListsNodes = scoreDetailNode.SelectNodes("./ul[@class='score-detail__list']/li");
                foreach (var node in detailListsNodes)
                {
                    var songTopNode = node.SelectSingleNode("./div/div[@class='song-info__top']");
                    var songBottomNode = node.SelectSingleNode("./div/div[@class='song-info__bottom']");

                    var playCountNode = songTopNode.SelectSingleNode("./div[@class='song-info__top__play-count']");
                    var scoreNode = songBottomNode.SelectSingleNode("./div[@class='song-info__score']");
                    var achieveNode = songBottomNode.SelectSingleNode("./div[@class='score-detail__icon']/div/img[@alt='achieveimage']");

                    var achieveImgSrc = achieveNode.Attributes["src"].Value;
                    this.pageConnector.Logger?.Debug("Achieve Image Source: {ImageSource}", achieveImgSrc);

                    var playCount = int.Parse(numericRegex.Match(playCountNode.InnerText).Value);
                    var score = int.Parse(numericRegex.Match(scoreNode.InnerText).Value);
                    var achieve = new Regex("achieve[1-4].png").Match(achieveImgSrc).Value switch
                    {
                        "achieve1.png" => Achieve.Clear,
                        "achieve2.png" => Achieve.Missless,
                        "achieve3.png" => Achieve.FullCombo,
                        "achieve4.png" => Achieve.AllMarvelous,
                        _ => Achieve.NoAchieve
                    };

                    playCounts.Add(playCount);
                    scores.Add(score);
                    achieves.Add(achieve);
                }

                result = new Music(musicArg, artist, playCounts.ToArray(), scores.ToArray(), achieves.ToArray());        
            }
            catch (Exception ex)
            {
                this.pageConnector.Logger?.Error(ex.Message);

                return null;
            }

            this.pageConnector.Logger?.Information(Localization.Fetcher.DataFetched2, 
                Localization.Data.Music,
                result);

            return result;
        }

        public async Task<string> FetchMusicImageAsync(string musicId)
        {
            // Connect to the page and get an HTML document.
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            string? result = null;

            try
            {
                this.pageConnector.Logger?.Information("Trying to save music image to {Path}", Directories.RecordImage);

                if (!Directory.Exists(Directories.RecordImage))
                {
                    Directory.CreateDirectory(Directories.RecordImage);

                    this.pageConnector.Logger?.Information("No directory found. Create new directory: {Directory}", Directories.RecordImage);
                }

                var fileName = musicId + ".png";
                var imageUrl = new Uri(new Uri(this.ImageUrl), fileName);

                using (var msg = new HttpRequestMessage(HttpMethod.Get, imageUrl))
                {
                    msg.Headers.Referrer = new Uri(this.Url);

                    this.pageConnector.Logger?.Debug("Set Referrer as {Referrer} and send request.", msg.Headers.Referrer);

                    using (var request = await this.pageConnector.Client.SendAsync(msg).ConfigureAwait(false))
                    using (var fs = new FileStream(Path.Combine(Directories.RecordImage, fileName), FileMode.Create, FileAccess.Write))
                    {
                        await request.Content.CopyToAsync(fs);
                        result = Path.GetFullPath(Path.Combine(Directories.RecordImage, fileName));

                        this.pageConnector.Logger?.Information("Music({Id}) image has been saved at {Path}",
                            musicId, Path.GetFullPath(Directories.RecordImage));
                    }
                }
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
