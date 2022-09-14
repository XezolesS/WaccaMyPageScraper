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

        public override async Task<bool> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
        {
            // Connect to the page and get an HTML document.
            if (!this.pageConnector.IsLoggedOn())
            {
                // Logging and Reporting progress.
                this.pageConnector.Logger?.Error(Localization.Fetcher.NotLoggedIn);

                progressText.Report(Localization.Connector.LoggedOff);
                progressPercent.Report(0);

                return false;
            }

            if (!Directory.Exists(ResourceDirectory))
            {
                Directory.CreateDirectory(ResourceDirectory);

                this.pageConnector.Logger?.Information(Localization.Fetcher.NoDirectory, Path.GetFullPath(ResourceDirectory));
            }

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

                        this.pageConnector.Logger?.Debug(Localization.Fetcher.SetReferrer, msg.Headers.Referrer);

                        using (var request = await this.pageConnector.Client.SendAsync(msg).ConfigureAwait(false))
                        using (var fs = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
                        {
                            if (!request.IsSuccessStatusCode)
                            {
                                this.pageConnector.Logger?.Error(Localization.Fetcher.ConnectionError);
                                continue;
                            }

                            await request.Content.CopyToAsync(fs);

                            this.pageConnector.Logger?.Information(Localization.Fetcher.DataSaved,
                                Localization.Data.RateIcon, Path.GetFullPath(imagePath));
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
