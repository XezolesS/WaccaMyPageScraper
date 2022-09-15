using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Resources;

namespace WaccaMyPageScraper.FetcherActions
{
    internal sealed class TrophyIconsFetcherAction : FetcherAction<string>
    {
        protected override string Url => "https://wacca.marv-games.jp/img/trophy/";

        public TrophyIconsFetcherAction(Fetcher fetcher) : base(fetcher) { }

        /// <summary>
        /// Fetch all trophy icons.
        /// </summary>
        /// <param name="progressText"></param>
        /// <param name="progressPercent"></param>
        /// <param name="args">None</param>
        /// <returns>A path where the icon saved at. null if it's failed.</returns>
        public override async Task<string?> FetchAsync(IProgress<string> progressText, IProgress<int> progressPercent, params object?[] args)
        {
            // Connect to the page and get an HTML document.
            if (!this._fetcher.IsLoggedOn())
            {
                // Logging and Reporting progress.
                this._fetcher.Logger?.Error(Localization.Fetcher.NotLoggedIn);

                progressText.Report(Localization.Fetcher.LoggedOff);
                progressPercent.Report(0);

                return null;
            }

            if (!Directory.Exists(Directories.Resources))
            {
                Directory.CreateDirectory(Directories.Resources);

                this._fetcher.Logger?.Information(Localization.Fetcher.NoDirectory, Path.GetFullPath(Directories.Resources));
            }

            try
            {
                for (int categroy = 1; categroy <= 6; categroy++)
                {
                    for (int rarity = 1; rarity <= 4; rarity++)
                    {
                        var fileName = $"{categroy}_{rarity}.png";
                        var imagePath = Path.Combine(Directories.Resources, fileName);
                        var imageUrl = new Uri(new Uri(this.Url), fileName);

                        using (var msg = new HttpRequestMessage(HttpMethod.Get, imageUrl))
                        {
                            msg.Headers.Referrer = new Uri("https://wacca.marv-games.jp/web/trophy");

                            this._fetcher.Logger?.Debug(Localization.Fetcher.SetReferrer, msg.Headers.Referrer);

                            using (var request = await this._fetcher.Client.SendAsync(msg).ConfigureAwait(false))
                            using (var fs = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
                            {
                                if (!request.IsSuccessStatusCode)
                                {
                                    this._fetcher.Logger?.Error(Localization.Fetcher.ConnectionError);
                                    continue;
                                }

                                await request.Content.CopyToAsync(fs);

                                this._fetcher.Logger?.Information(Localization.Fetcher.DataSaved,
                                    $"{Localization.Data.TrophyIcon}({fileName})", Path.GetFullPath(imagePath));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._fetcher.Logger?.Error(ex.Message);

                return null;
            }

            return Directories.Resources;
        }
    }
}
