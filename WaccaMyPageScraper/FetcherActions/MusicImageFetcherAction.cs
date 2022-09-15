using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Resources;

namespace WaccaMyPageScraper.FetcherActions
{
    internal sealed class MusicImageFetcherAction : FetcherAction<string>
    {
        protected override string Url => "https://wacca.marv-games.jp/img/music/";

        public MusicImageFetcherAction(Fetcher fetcher) : base(fetcher) { }

        /// <summary>
        /// Fetch jacket image of the music.
        /// </summary>
        /// <param name="progressText"></param>
        /// <param name="progressPercent"></param>
        /// <param name="args"><see cref="MusicMetadata"/> is needed.</param>
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

            if (args[0] is null || args[0] is not MusicMetadata)
            {
                this._fetcher.Logger?.Error(Localization.Fetcher.NoArgument,
                    MethodBase.GetCurrentMethod()?.Name,
                    typeof(MusicMetadata));

                return null;
            }

            if (!Directory.Exists(Directories.Resources))
            {
                Directory.CreateDirectory(Directories.Resources);

                this._fetcher.Logger?.Information(Localization.Fetcher.NoDirectory, Path.GetFullPath(Directories.Resources));
            }

            string? result = null;

            try
            {
                var musicArg = args[0] as MusicMetadata;

                var fileName = musicArg.Id + ".png";
                var imageUrl = new Uri(new Uri(this.Url), fileName);

                using (var msg = new HttpRequestMessage(HttpMethod.Get, imageUrl))
                {
                    msg.Headers.Referrer = this.BaseUrl;

                    this._fetcher.Logger?.Debug(Localization.Fetcher.SetReferrer, msg.Headers.Referrer);

                    using (var request = await this._fetcher.Client.SendAsync(msg).ConfigureAwait(false))
                    using (var fs = new FileStream(Path.Combine(Directories.RecordImage, fileName), FileMode.Create, FileAccess.Write))
                    {
                        if (!request.IsSuccessStatusCode)
                        {
                            this._fetcher.Logger?.Error(Localization.Fetcher.ConnectionError);

                            return null;
                        }

                        await request.Content.CopyToAsync(fs);
                        result = Path.GetFullPath(Directories.RecordImage);

                        this._fetcher.Logger?.Information(Localization.Fetcher.DataSaved,
                            $"{Localization.Data.MusicImage}({fileName})", result);
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
