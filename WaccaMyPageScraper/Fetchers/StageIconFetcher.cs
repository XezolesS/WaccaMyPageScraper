using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Resources;

namespace WaccaMyPageScraper.Fetchers
{
    public class StageIconFetcher : Fetcher<bool>
    {
        protected override string Url => "https://wacca.marv-games.jp/img/trophy/";

        public StageIconFetcher(PageConnector pageConnector) : base(pageConnector) { }

        public override async Task<bool> FetchAsync(params object?[] args)
        {
            // Connect to the page and get an HTML document.
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return false;
            }

            if (!Directory.Exists(DataFilePath.StageImage))
            {
                Directory.CreateDirectory(DataFilePath.StageImage);

                this.pageConnector.Logger?.Information("No directory found. Create new directory: {Directory}",
                    Path.GetFullPath(DataFilePath.StageImage));
            }

            try
            {
                for (int stage = 1; stage <= 14; stage++)
                {
                    for (int grade = 1; grade <= 3; grade++)
                    {
                        var fileName = $"stage_icon_{stage}_{grade}.png";
                        var imagePath = Path.Combine(DataFilePath.StageImage, fileName);
                        var imageUrl = new Uri(new Uri(this.Url), fileName);

                        using (var msg = new HttpRequestMessage(HttpMethod.Get, imageUrl))
                        {
                            msg.Headers.Referrer = new Uri("https://wacca.marv-games.jp/web/stageup");

                            this.pageConnector.Logger?.Debug("Set Referrer as {Referrer} and send request.", msg.Headers.Referrer);

                            using (var request = await this.pageConnector.Client.SendAsync(msg).ConfigureAwait(false))
                            using (var fs = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
                            {
                                await request.Content.CopyToAsync(fs);

                                this.pageConnector.Logger?.Information("Stage icon has been saved at {Path}", Path.GetFullPath(imagePath));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.pageConnector.Logger?.Error(ex.Message);

                return false;
            }

            return true;
        }
    }
}
