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
    public sealed class MusicMetadataFetcher : Fetcher<MusicMetadata[]>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/music";

        public MusicMetadataFetcher(PageConnector pageConnector) : base(pageConnector) { }

        /// <summary>
        /// Fetch musics listed on My Page.
        /// </summary>
        /// <param name="args">No argument needed.</param>
        /// <returns>List of musics listed on My Page in array of <see cref="MusicMetadata"/>s.</returns>
        public override async Task<MusicMetadata[]?> FetchAsync(params object?[] args)
        {
            // Connect to the page and get an HTML document.
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return null;
            }

            this.pageConnector.Logger?.Information("Trying to connect to {URL}", Url);

            var response = await this.pageConnector.Client.GetStringAsync(this.Url).ConfigureAwait(false);
            List<MusicMetadata> result = new List<MusicMetadata>();

            if (string.IsNullOrEmpty(response))
            {
                this.pageConnector.Logger?.Error("Error occured while connecting to the page!");

                return null;
            }

            this.pageConnector.Logger?.Information("Connection successful");

            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(response);

                var scoreListNode = document.DocumentNode.SelectSingleNode("//section[@class='playdata']/div[@class='contents-wrap']/div[@class='playdata__score-list']/ul");
                var musicItemNodes = scoreListNode.SelectNodes("./li");

                // Fetch genre data
                this.pageConnector.Logger?.Information("Fetching genre data...");

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

                this.pageConnector.Logger?.Information("Genre has successfully fetched.");

                // Fetch music data
                int count = 0;
                foreach (var node in musicItemNodes)
                {
                    string id = node.SelectSingleNode("./div/form/input").Attributes["value"].Value;
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
                        node.Attributes["data-rank_normal_level"].Value.Replace(".1", "+"),
                        node.Attributes["data-rank_hard_level"].Value.Replace(".1", "+"),
                        node.Attributes["data-rank_expert_level"].Value.Replace(".1", "+"),
                        node.Attributes["data-rank_inferno_level"].Value.Replace(".1", "+"),
                    };

                    MusicMetadata music = new MusicMetadata(id, title, genre, levels);
                    result.Add(music);

                    this.pageConnector.Logger?.Information("Fetching music data... ({Count} out of {MusicTotal})", 
                        ++count, musicItemNodes.Count);
                    this.pageConnector.Logger?.Debug("Data: {Music}", music);
                }
            }
            catch (Exception ex)
            {
                this.pageConnector.Logger?.Error(ex.Message);

                return null;
            }

            this.pageConnector.Logger?.Information("Successfully fetched {ResultCount} of musics.", result.Count);

            return result.ToArray();
        }
    }
}
