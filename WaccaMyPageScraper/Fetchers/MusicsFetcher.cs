using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;

namespace WaccaMyPageScraper.Fetchers
{
    public class MusicsFetcher : IFetcher<Music[]>
    {
        private readonly static string Url = "https://wacca.marv-games.jp/web/music";

        private readonly PageConnector pageConnector;

        public MusicsFetcher(PageConnector pageConnector)
        {
            this.pageConnector = pageConnector;
        }

        public async Task<Music[]> FetchAsync()
        {
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            var response = await pageConnector.GetStringAsync(Url);
            List<Music> result = new List<Music>();

            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(response);

                var musicItemNodes = document.DocumentNode.SelectNodes("//section[@class='playdata']/div[@class='contents-wrap']/div[@class='playdata__score-list']/ul/li");

                pageConnector.Logger?.Debug("Music found: {MusicCount}", musicItemNodes.Count);

                foreach (var node in musicItemNodes)
                {
                    int id = int.Parse(node.SelectSingleNode("./div/form/input").Attributes["value"].Value);
                    string title = node.SelectSingleNode("./div/a/div/div[@class='playdata__score-list__song-info__name']").InnerText;
                    string[] levels = new string[]
                    {
                        node.Attributes["data-rank_normal_level"].Value,
                        node.Attributes["data-rank_hard_level"].Value,
                        node.Attributes["data-rank_expert_level"].Value,
                        node.Attributes["data-rank_inferno_level"].Value,
                    };

                    result.Add(new Music { Id = id, Title = title, Levels = levels });
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
