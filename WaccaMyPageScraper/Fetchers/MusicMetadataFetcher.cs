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
        public override async Task<MusicMetadata[]?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
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

            this.pageConnector.Logger?.Information(Localization.Fetcher.Connecting, Url);

            var response = await this.pageConnector.Client.GetStringAsync(this.Url).ConfigureAwait(false);
            List<MusicMetadata> result = new List<MusicMetadata>();

            if (string.IsNullOrEmpty(response))
            {
                this.pageConnector.Logger?.Error(Localization.Fetcher.ConnectionError);

                return null;
            }

            this.pageConnector.Logger?.Information(Localization.Fetcher.Connected);

            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(response);

                var scoreListNode = document.DocumentNode.SelectSingleNode("//section[@class='playdata']/div[@class='contents-wrap']/div[@class='playdata__score-list']/ul");
                var musicItemNodes = scoreListNode.SelectNodes("./li");

                // Reporting progress.
                progressText.Report(string.Format(Localization.Fetcher.Fetching, 
                    Localization.Data.StageMetadata));
                progressPercent.Report(0);

                // Fetch genre data
                this.pageConnector.Logger?.Information(Localization.Fetcher.Fetching,
                    Localization.Data.Genre);

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

                this.pageConnector.Logger?.Information(Localization.Fetcher.DataFetched1,
                    Localization.Data.Genre);

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

                    MusicMetadata meta = new MusicMetadata(id, title, genre, levels);
                    result.Add(meta);
                    ++count;

                    // Logging and Reporting progress.
                    this.pageConnector.Logger?.Information(Localization.Fetcher.FetchingMany, 
                        Localization.Data.MusicMetadata, count, musicItemNodes.Count);
                    this.pageConnector.Logger?.Debug("Data: {MusicMetadata}", meta);

                    progressText.Report(string.Format(Localization.Fetcher.FetchingProgress,
                       count, musicItemNodes.Count, $"{Localization.Data.MusicMetadata}({meta.Title})"));
                    progressPercent.Report(CalculatePercent(count, musicItemNodes.Count));
                }
            }
            catch (Exception ex)
            {
                this.pageConnector.Logger?.Error(ex.Message);

                return null;
            }

            // Logging and Reporting progress.
            this.pageConnector.Logger?.Information(Localization.Fetcher.DataFetched3,
                result.Count, Localization.Data.MusicMetadata);
            progressText.Report(string.Format(Localization.Fetcher.DataFetched3, 
                result.Count, Localization.Data.MusicMetadata));

            return result.ToArray();
        }
    }
}
