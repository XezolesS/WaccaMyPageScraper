using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;

namespace WaccaMyPageScraper.Fetchers
{
    public sealed class TrophiesFetcher : Fetcher<Trophy[]>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/trophy/index/get";
        private string DescriptionUrl => "https://wacca.marv-games.jp/web/modal/trophy/conditionsConfirm";

        public TrophiesFetcher(PageConnector pageConnector) : base(pageConnector) { }

        /// <summary>
        /// Fetch player's trophies.
        /// </summary>
        /// <param name="args">No argument needed.</param>
        /// <returns>List of trophies listed on My Page in array of <see cref="Trophy"/>s.</returns>
        public override async Task<Trophy[]?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
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

            List<Trophy> result = new List<Trophy>();

            // Fetch all trophies from season 1 to season 3
            for (int season = 1; season <= 3; season++)
            {
                // Logging and Reporting progress.
                string textSeasonTrophy = $"{Localization.Data.Trophy}({Localization.Data.Season} {season})";

                this.pageConnector.Logger?.Information(Localization.Fetcher.Connecting, Url);
                this.pageConnector.Logger?.Debug(Localization.Fetcher.Fetching, textSeasonTrophy);

                progressText.Report(string.Format(Localization.Fetcher.Fetching, textSeasonTrophy));
                progressPercent.Report(0);

                var parameters = new Dictionary<string, string> { { "seasonId", season.ToString() } };
                var encodedContent = new FormUrlEncodedContent(parameters);

                var response = await this.pageConnector.Client.PostAsync(this.Url, encodedContent).ConfigureAwait(false);
                var responseContent = await response.Content.ReadAsStringAsync();

                var trophyJsonObject = JObject.Parse(responseContent);
                if (trophyJsonObject is null)
                {
                    // Logging and Reporting progress.
                    this.pageConnector.Logger?.Warning(Localization.Fetcher.FetchingFail, textSeasonTrophy);

                    progressText.Report(string.Format(Localization.Fetcher.FetchingFail, textSeasonTrophy));
                    progressPercent.Report(0);

                    continue;
                }

                int count = 0;
                IList<JToken> trophies = trophyJsonObject["trophyMasterList"].Children().ToList();
                foreach (var trophy in trophies)
                {
                    var resultItem = trophy.ToObject<Trophy>();

                    // Fetch trophy's description
                    this.pageConnector.Logger?.Debug("Fetching description for trophy ID: {TrophyId}", resultItem.Id);

                    var descResponse = await this.pageConnector.Client
                        .GetStringAsync($"{DescriptionUrl}?trid={resultItem.Id}")
                        .ConfigureAwait(false);
                    var descDocument = new HtmlDocument();
                    descDocument.LoadHtml(descResponse);

                    var descriptionNode = descDocument.DocumentNode.SelectSingleNode("//p[@class='description']/span");
                    var description = descriptionNode.InnerText;

                    resultItem.Description = description;

                    result.Add(resultItem);
                    ++count;

                    // Logging and Reporting progress.
                    this.pageConnector.Logger?.Information(Localization.Fetcher.FetchingMany,
                        textSeasonTrophy, count, trophies.Count);
                    this.pageConnector.Logger?.Debug("Data: {Trophy}", resultItem);

                    progressText.Report(string.Format(Localization.Fetcher.FetchingProgress,
                        count, trophies.Count, textSeasonTrophy));
                    progressPercent.Report(CalculatePercent(count, trophies.Count));
                }
            }

            // Logging and Reporting progress.
            this.pageConnector.Logger?.Information(Localization.Fetcher.DataFetched3,
                result.Count, Localization.Data.Trophy);
            progressText.Report(string.Format(Localization.Fetcher.DataFetched3,
                result.Count, Localization.Data.Trophy));

            return result.ToArray();
        }
    }
}
