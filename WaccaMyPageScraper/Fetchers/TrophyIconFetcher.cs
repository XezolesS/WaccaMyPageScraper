using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Resources;

namespace WaccaMyPageScraper.Fetchers
{
    public sealed class TrophyIconFetcher : Fetcher<bool>
    {
        protected override string Url => "https://wacca.marv-games.jp/img/trophy/";

        public TrophyIconFetcher(PageConnector pageConnector) : base(pageConnector) { }

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

            if (!Directory.Exists(Directories.TrophyImage))
            {
                Directory.CreateDirectory(Directories.TrophyImage);

                this.pageConnector.Logger?.Information(Localization.Fetcher.NoDirectory, Path.GetFullPath(Directories.TrophyImage));
            }

            try
            {
                for (int categroy = 1; categroy <= 6; categroy++)
                {
                    for (int rarity = 1; rarity <= 4; rarity++)
                    {
                        var fileName = $"{categroy}_{rarity}.png";
                        var imagePath = Path.Combine(Directories.TrophyImage, fileName);
                        var imageUrl = new Uri(new Uri(this.Url), fileName);

                        using (var msg = new HttpRequestMessage(HttpMethod.Get, imageUrl))
                        {
                            msg.Headers.Referrer = new Uri("https://wacca.marv-games.jp/web/trophy");

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
                                    Localization.Data.TrophyIcon,
                                    Path.GetFullPath(imagePath));
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
