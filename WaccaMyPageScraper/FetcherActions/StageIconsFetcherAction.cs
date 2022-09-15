using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Resources;

namespace WaccaMyPageScraper.FetcherActions
{
    internal sealed class StageIconsFetcherAction : FetcherAction<string>
    {
        protected override string Url => "https://wacca.marv-games.jp/img/web/stage/rank/";

        public StageIconsFetcherAction(Fetcher fetcher) : base(fetcher) { }

        /// <summary>
        /// Fetch all stage icons.
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
                for (int stage = 1; stage <= 14; stage++)
                {
                    for (int grade = 1; grade <= 3; grade++)
                    {
                        var fileName = $"stage_icon_{stage}_{grade}.png";
                        var imagePath = Path.Combine(Directories.Resources, fileName);
                        var imageUrl = new Uri(new Uri(this.Url), fileName);

                        using (var msg = new HttpRequestMessage(HttpMethod.Get, imageUrl))
                        {
                            msg.Headers.Referrer = new Uri("https://wacca.marv-games.jp/web/stageup");

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
                                    $"{Localization.Data.StageIcon}({fileName})", Path.GetFullPath(imagePath));
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
