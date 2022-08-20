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
    public class TrophiesFetcher : IFetcher<Trophy[]>
    {
        private readonly static string Url = "https://wacca.marv-games.jp/web/trophy/index/get";
        private readonly static string DescriptionUrl = "https://wacca.marv-games.jp/web/modal/trophy/conditionsConfirm";

        private readonly PageConnector pageConnector;

        public TrophiesFetcher(PageConnector pageConnector)
        {
            this.pageConnector = pageConnector;
        }

        public async Task<Trophy[]> FetchAsync(params object?[] args)
        {
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            List<Trophy> result = new List<Trophy>();

            // Fetch all trophies from season 1 to season 3
            for (int season = 1; season <= 3; season++)
            {
                var parameters = new Dictionary<string, string> { { "seasonId", season.ToString() } };
                var encodedContent = new FormUrlEncodedContent(parameters);

                var response = await this.pageConnector.PostAsync(Url, encodedContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                var trophyJsonObject = JObject.Parse(responseContent);
                if (trophyJsonObject is null)
                {
                    this.pageConnector.Logger?.Warning("Failed to retrieve trophy data. Skip season {season}", season);

                    continue;
                }

                IList<JToken> trophies = trophyJsonObject["trophyMasterList"].Children().ToList();
                foreach (var trophy in trophies)
                {
                    var resultItem = trophy.ToObject<Trophy>();

                    // Fetch trophy's description
                    var descResponse = await this.pageConnector.GetStringAsync($"{DescriptionUrl}?trid={resultItem.Id}");

                    var descDocument = new HtmlDocument();
                    descDocument.LoadHtml(descResponse);

                    var descriptionNode = descDocument.DocumentNode.SelectSingleNode("//p[@class='description']/span");
                    var description = descriptionNode.InnerText;

                    resultItem.Description = description;

                    result.Add(resultItem);

                    this.pageConnector.Logger?.Debug("Fetched trophy data: {Trophy}", resultItem);
                }
            }
            
            return result.ToArray();
        }
    }
}
