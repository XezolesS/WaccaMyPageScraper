using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Resources;

namespace WaccaMyPageScraper.FetcherActions
{
    internal sealed class PlayerIconFetcherAction : FetcherAction<string>
    {
        protected override string Url => "https://wacca.marv-games.jp/web/player";

        public PlayerIconFetcherAction(Fetcher fetcher) : base(fetcher) { }

        /// <summary>
        /// Fetch player's icon.
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

            this._fetcher.Logger?.Information(Localization.Fetcher.Connecting, Url);

            var response = await this._fetcher.Client.GetStringAsync(this.Url).ConfigureAwait(false);
            string? result = null;

            if (string.IsNullOrEmpty(response))
            {
                this._fetcher.Logger?.Error(Localization.Fetcher.ConnectionError);

                return null;
            }

            this._fetcher.Logger?.Information(Localization.Fetcher.Connected);

            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(response);

                var playerIconNode = document.DocumentNode.SelectSingleNode("//div[@class='icon__image']/img");
                var playerIconSrc = playerIconNode.Attributes["src"].Value;

                if (!Directory.Exists(Directories.PlayerImage))
                {
                    Directory.CreateDirectory(Directories.PlayerImage);

                    this._fetcher.Logger?.Information(Localization.Fetcher.NoDirectory, Path.GetFullPath(Directories.Player));
                }

                var imageUrl = new Uri(this.BaseUrl, playerIconSrc);
                this._fetcher.Logger?.Information(Localization.Fetcher.Connecting, imageUrl);

                using (var msg = new HttpRequestMessage(HttpMethod.Get, imageUrl))
                {
                    msg.Headers.Referrer = new Uri("https://wacca.marv-games.jp/web/player");

                    this._fetcher.Logger?.Debug(Localization.Fetcher.SetReferrer, msg.Headers.Referrer);

                    using (var request = await this._fetcher.Client.SendAsync(msg).ConfigureAwait(false))
                    using (var fs = new FileStream(Directories.PlayerIcon, FileMode.Create, FileAccess.Write))
                    {
                        if (!request.IsSuccessStatusCode)
                        {
                            this._fetcher.Logger?.Error(Localization.Fetcher.ConnectionError);

                            return null;
                        }

                        await request.Content.CopyToAsync(fs);
                        result = Path.GetFullPath(Directories.PlayerIcon);

                        this._fetcher.Logger?.Information(Localization.Fetcher.DataSaved,
                                Localization.Data.PlayerIcon, result);
                    }
                }
            }
            catch (Exception ex)
            {
                this._fetcher.Logger?.Error(ex.Message);

                return null;
            }

            return result;
        }
    }
}
