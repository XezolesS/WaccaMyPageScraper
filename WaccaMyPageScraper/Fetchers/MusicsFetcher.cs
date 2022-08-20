using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;

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

        public async Task<Music[]> FetchAsync(params object?[] args)
        {
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            var response = await pageConnector.GetStringAsync(Url);
            List<Music> result = new List<Music>();

            if (string.IsNullOrEmpty(response))
            {
                this.pageConnector.Logger?.Error("Error occured while connecting to the page!");

                return null;
            }

            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(response);

                var scoreListNode = document.DocumentNode.SelectSingleNode("//section[@class='playdata']/div[@class='contents-wrap']/div[@class='playdata__score-list']/ul");

                var musicItemNodes = scoreListNode.SelectNodes("./li");
                pageConnector.Logger?.Information("Music found: {MusicCount}", musicItemNodes.Count);

                // Fetch genre data
                var genreListsNodes = scoreListNode.SelectNodes("./h2");
                var genreOffsets = new Dictionary<Genre, int>();
                foreach (var node in genreListsNodes)
                {
                    if (!(node.HasClass("filter-genre") && node.HasClass("last")))
                        continue;

                    var genreTitle = node.SelectSingleNode("./div").InnerText switch
                    {
                        "アニメ／ＰＯＰ" => Genre.AnimePop,
                        "ボーカロイド" => Genre.Vocaloid,
                        "東方" => Genre.Touhou,
                        "2.5次元" => Genre.TwoPointFive,
                        "バラエティ" => Genre.Variety,
                        "オリジナル" => Genre.Original,
                        "HARDCORE TANO*C" => Genre.TanoC,
                        _ => Genre.Unknown
                    };
                    var genreOffset = (int)double.Parse(node.Attributes["data-sequence_number"].Value);

                    genreOffsets.Add(genreTitle, genreOffset);
                }

                foreach (var node in musicItemNodes)
                {
                    int id = int.Parse(node.SelectSingleNode("./div/form/input").Attributes["value"].Value);
                    string title = node.SelectSingleNode("./div/a/div/div[@class='playdata__score-list__song-info__name']").InnerText;
                    Genre genre = int.Parse(node.Attributes["data-sequence_number"].Value) switch
                    {
                        var n when n < genreOffsets[Genre.AnimePop] => Genre.AnimePop,
                        var n when n < genreOffsets[Genre.Vocaloid] => Genre.Vocaloid,
                        var n when n < genreOffsets[Genre.Touhou] => Genre.Touhou,
                        var n when n < genreOffsets[Genre.TwoPointFive] => Genre.TwoPointFive,
                        var n when n < genreOffsets[Genre.Variety] => Genre.Variety,
                        var n when n < genreOffsets[Genre.Original] => Genre.Original,
                        var n when n < genreOffsets[Genre.TanoC] => Genre.TanoC,
                        _ => Genre.Unknown
                    };
                    string[] levels = new string[]
                    {
                        node.Attributes["data-rank_normal_level"].Value,
                        node.Attributes["data-rank_hard_level"].Value,
                        node.Attributes["data-rank_expert_level"].Value,
                        node.Attributes["data-rank_inferno_level"].Value,
                    };

                    result.Add(new Music { Id = id, Title = title, Genre = genre, Levels = levels });
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
