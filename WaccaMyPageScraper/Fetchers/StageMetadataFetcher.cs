using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Fetchers
{
    public sealed class StageMetadataFetcher : Fetcher<StageMetadata[]>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/stageup";

        public StageMetadataFetcher(PageConnector pageConnector) : base(pageConnector) { }

        /// <summary>
        /// Fetch stages listed on My Page.
        /// </summary>
        /// <param name="args">No argument needed.</param>
        /// <returns>List of stages listed on My Page in array of <see cref="StageMetadata"/>s.</returns>
        public override async Task<StageMetadata[]?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
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
            List<StageMetadata> result = new List<StageMetadata>();

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

                var stageListNode = document.DocumentNode.SelectSingleNode("//section[@class='stageup']/ul[@class='stageup__list']");
                var stageItemNodes = stageListNode.SelectNodes("./form/a[@class='stageup__list__link']");

                int count = 0;
                foreach (var node in stageItemNodes)
                {
                    var stageNameNode = node.SelectSingleNode("./li/div/div[@class='stageup__list__top__name']");
                    var stageIconNode = node.SelectSingleNode("./li/div[@class='stageup__list__course-icon']/img");

                    StageMetadata meta;
                    if (stageIconNode is null)
                    {
                        int id = int.Parse(node.Attributes["value"].Value);
                        var name = stageNameNode.InnerText;
                        meta = new StageMetadata(id, name, StageGrade.NotCleared);
                    }
                    else
                    {
                        var stageIconImgSrc = stageIconNode.Attributes["src"].Value;

                        this.pageConnector.Logger?.Debug("Icon Image Source: {UserIconImgNode}", stageIconImgSrc);

                        var stageIconFileName = new Regex(@"stage_icon_[0-9]+_[1-3].png").Match(stageIconImgSrc).Value;
                        var stageIconNumbers = new Regex("[0-9]+_[1-3]").Match(stageIconFileName).Value.Split('_');

                        int id = int.Parse(stageIconNumbers[0]);
                        StageGrade grade = (StageGrade)int.Parse(stageIconNumbers[1]);

                        meta = new StageMetadata(id, grade);
                    }

                    result.Add(meta);
                    ++count;

                    // Logging and Reporting progress.
                    this.pageConnector.Logger?.Information(Localization.Fetcher.FetchingMany,
                        Localization.Data.StageMetadata, count, stageItemNodes.Count);
                    this.pageConnector.Logger?.Debug("Data: {MusicMetadata}", meta);

                    progressText.Report(string.Format(Localization.Fetcher.FetchingProgress,
                       count, stageItemNodes.Count, $"{Localization.Data.StageMetadata}({meta.Name})"));
                    progressPercent.Report(CalculatePercent(count, stageItemNodes.Count));
                }
            }
            catch (Exception ex)
            {
                this.pageConnector.Logger?.Error(ex.Message);

                return null;
            }

            // Logging and Reporting progress.
            this.pageConnector.Logger?.Information(Localization.Fetcher.DataFetched3,
                result.Count, Localization.Data.StageMetadata);
            progressText.Report(string.Format(Localization.Fetcher.DataFetched3,
                result.Count, Localization.Data.StageMetadata));

            return result.ToArray();
        }
    }
}
