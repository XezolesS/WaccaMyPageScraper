using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Fetchers
{
    public sealed class RateIconFetcher : Fetcher<bool>
    {
        protected override string Url => "https://wacca.marv-games.jp/img/web/music/rate_icon/";

        private static readonly string ResourceDirectory = "rsrc/";

        public RateIconFetcher(PageConnector pageConnector) : base(pageConnector) { }

        public override async Task<bool> FetchAsync(params object?[] args)
        {
            // Connect to the page and get an HTML document.
            if (!this.pageConnector.IsLoggedOn())
            {
                this.pageConnector.Logger?.Error("Connector is not logged in to the page!");

                return false;
            }

            if (!Directory.Exists(ResourceDirectory))
                Directory.CreateDirectory(ResourceDirectory);

            try
            {
                for (int i = 1; i <= (int)Rate.SSS_Plus + 1; i++)
                {
                    var fileName = $"rate_{i}.png";
                    var imagePath = Path.Combine(ResourceDirectory, fileName);
                    var imageUrl = new Uri(new Uri(this.Url), fileName);

                    using (var msg = new HttpRequestMessage(HttpMethod.Get, imageUrl))
                    {
                        msg.Headers.Referrer = this.BaseUrl;

                        this.pageConnector.Logger?.Debug("Set Referrer as {Referrer} and send request.", msg.Headers.Referrer);

                        using (var request = await this.pageConnector.Client.SendAsync(msg).ConfigureAwait(false))
                        using (var fs = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
                        {
                            await request.Content.CopyToAsync(fs);

                            this.pageConnector.Logger?.Information("Player icon has been saved at {Path}", Path.GetFullPath(imagePath));
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
