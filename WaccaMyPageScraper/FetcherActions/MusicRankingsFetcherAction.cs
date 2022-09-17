using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;

namespace WaccaMyPageScraper.FetcherActions
{
    internal sealed class MusicRankingsFetcherAction : FetcherAction<MusicRankings>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/ranking/musicHighScore/detail";
        
        public MusicRankingsFetcherAction(Fetcher fetcher) : base(fetcher) { }

        /// <summary>
        /// Fetch player's rankings of the music.
        /// </summary>
        /// <param name="args"><see cref="MusicMetadata"/> is needed.</param>
        /// <returns>Fetched player's record of given <see cref="MusicMetadata"/> in <see cref="Music"/>.</returns>
        public override async Task<MusicRankings?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
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

            if (args[0] is null || args[0] is not MusicMetadata)
            {
                this._fetcher.Logger?.Error(Localization.Fetcher.NoArgument,
                    MethodBase.GetCurrentMethod().Name,
                    typeof(MusicMetadata));

                return null;
            }

            var musicArg = args[0] as MusicMetadata;

            var numericRegex = new Regex("[0-9,+]+");

            List<int> rankings = new List<int>();
            for (int i = 1; i <= musicArg.Levels.Length; i++)
            {
                if (i == 4 && !musicArg.HasInferno())
                    continue;

                this._fetcher.Logger?.Information(Localization.Fetcher.Connecting, Url);

                var parameters = new Dictionary<string, string> {
                    { "rankCategory", i.ToString() },
                    { "musicId", musicArg.Id.ToString() } 
                };
                var encodedContent = new FormUrlEncodedContent(parameters);

                var response = await this._fetcher.Client.PostAsync(this.Url, encodedContent).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    this._fetcher.Logger?.Error(Localization.Fetcher.ConnectionError);

                    return null;
                }

                this._fetcher.Logger?.Information(Localization.Fetcher.Connected);

                try
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Check response content HTML to find out if it's an error page.
                    var document = new HtmlDocument();
                    document.LoadHtml(responseContent);

                    var rankingNode = document.DocumentNode.SelectSingleNode("//div[@class='ranking__score__rank top-rank']");
                    var rankingImageNode = rankingNode.SelectSingleNode("(./img)[last()]");

                    // Fetch ranking
                    this._fetcher.Logger?.Information(Localization.Fetcher.Fetching,
                        Localization.Data.MusicRanking + $"({musicArg.Id}:{i})");

                    var ranking = -1;
                    if (rankingImageNode is null)
                    {
                        ranking = numericRegex.IsMatch(rankingNode.InnerText) ?
                            int.Parse(numericRegex.Match(rankingNode.InnerText).Value) :
                            -1;
                    }
                    else
                    {
                        ranking = int.Parse(
                            numericRegex.Match(rankingImageNode.Attributes["src"].Value)
                            .Value);
                    }

                    rankings.Add(ranking);
                }
                catch (Exception ex)
                {
                    this._fetcher.Logger?.Error(ex.Message);

                    return null;
                }
            }

            var result = new MusicRankings(musicArg, rankings.ToArray());

            this._fetcher.Logger?.Information(Localization.Fetcher.DataFetched2,
                Localization.Data.MusicRanking,
                result);

            return result;
        }
    }
}
