using HtmlAgilityPack;
using Serilog;
using System.Net;
using System.Net.Http.Headers;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;
using WaccaMyPageScraper.FetcherActions;

#pragma warning disable CS8603 // Possible null reference return.

namespace WaccaMyPageScraper
{
    /// <summary>
    /// The primary class for fetching data from WACCA My Page.<br/>
    /// Acts as a interface for all of the fetchers.
    /// </summary>
    public class Fetcher : IDisposable
    {
        private static readonly string MyPageLoginExecUrl = "https://wacca.marv-games.jp/web/login/exec";

        private static readonly int MaxReconnectTries = 10;

        public string AimeId { get; private set; }

        public LoginStatus LoginStatus { get; internal set; }

        internal HttpClient Client { get; private set; }

        internal ILogger? Logger { get; private set; }

        public Fetcher(string aimeId, ILogger logger = null)
        {
            this.AimeId = aimeId;
            this.LoginStatus = LoginStatus.LoggedOff;

            this.Client = new HttpClient();
            this.Logger = logger;
        }

        public bool IsLoggedOn() => this.LoginStatus == LoginStatus.LoggedOn;

        public async Task<string> FetchAchieveIconsAsync() => await FetchAchieveIconsAsync(new Progress<string>(), new Progress<int>());
        public async Task<string> FetchAchieveIconsAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new AchieveIconsFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<Music> FetchMusicAsync(MusicMetadata musicMetadata) => await FetchMusicAsync(new Progress<string>(), new Progress<int>(), musicMetadata);
        public async Task<Music> FetchMusicAsync(IProgress<string> progressText, IProgress<int> progressPercent, MusicMetadata musicMetadata)
            => await new MusicFetcherAction(this).FetchAsync(progressText, progressPercent, musicMetadata);

        public async Task<string> FetchMusicImageAsync(MusicMetadata musicMetadata) => await FetchMusicImageAsync(new Progress<string>(), new Progress<int>(), musicMetadata);
        public async Task<string> FetchMusicImageAsync(IProgress<string> progressText, IProgress<int> progressPercent, MusicMetadata musicMetadata)
            => await new MusicImageFetcherAction(this).FetchAsync(progressText, progressPercent, musicMetadata);

        public async Task<MusicMetadata[]> FetchMusicMetadataAsync() => await FetchMusicMetadataAsync(new Progress<string>(), new Progress<int>());
        public async Task<MusicMetadata[]> FetchMusicMetadataAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new MusicMetadataFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<MusicRankings> FetchMusicRankingsAsync(MusicMetadata musicMetadata) => await FetchMusicRankingsAsync(new Progress<string>(), new Progress<int>(), musicMetadata);
        public async Task<MusicRankings> FetchMusicRankingsAsync(IProgress<string> progressText, IProgress<int> progressPercent, MusicMetadata musicMetadata)
            => await new MusicRankingsFetcherAction(this).FetchAsync(progressText, progressPercent, musicMetadata);

        public async Task<Player> FetchPlayerAsync() => await FetchPlayerAsync(new Progress<string>(), new Progress<int>());
        public async Task<Player> FetchPlayerAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new PlayerFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<string> FetchPlayerIconAsync() => await FetchPlayerIconAsync(new Progress<string>(), new Progress<int>());
        public async Task<string> FetchPlayerIconAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new PlayerIconFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<string> FetchRateIconsAsync() => await FetchRateIconsAsync(new Progress<string>(), new Progress<int>());
        public async Task<string> FetchRateIconsAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new RateIconsFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<Stage> FetchStageAsync(StageMetadata stageMetadata) => await FetchStageAsync(new Progress<string>(), new Progress<int>(), stageMetadata);
        public async Task<Stage> FetchStageAsync(IProgress<string> progressText, IProgress<int> progressPercent, StageMetadata stageMetadata)
            => await new StageFetcherAction(this).FetchAsync(progressText, progressPercent, stageMetadata);

        public async Task<string> FetchStageIconsAsync() => await FetchStageIconsAsync(new Progress<string>(), new Progress<int>());
        public async Task<string> FetchStageIconsAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new StageIconsFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<StageMetadata[]> FetchStageMetadataAsync() => await FetchStageMetadataAsync(new Progress<string>(), new Progress<int>());
        public async Task<StageMetadata[]> FetchStageMetadataAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new StageMetadataFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<StageRanking> FetchStageRankingAsync(StageMetadata stageMetadata) => await FetchStageRankingAsync(new Progress<string>(), new Progress<int>(), stageMetadata);
        public async Task<StageRanking> FetchStageRankingAsync(IProgress<string> progressText, IProgress<int> progressPercent, StageMetadata stageMetadata)
            => await new StageRankingFetcherAction(this).FetchAsync(progressText, progressPercent, stageMetadata);

        public async Task<TotalRpRanking> FetchTotalRpRankingAsync() => await FetchTotalRpRankingAsync(new Progress<string>(), new Progress<int>());
        public async Task<TotalRpRanking> FetchTotalRpRankingAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new TotalRpRankingFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<TotalScoreRankings> FetchTotalScoreRankingsAsync() => await FetchTotalScoreRankingsAsync(new Progress<string>(), new Progress<int>());
        public async Task<TotalScoreRankings> FetchTotalScoreRankingsAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new TotalScoreRankingsFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<Trophy[]> FetchTrophiesAsync() => await FetchTrophiesAsync(new Progress<string>(), new Progress<int>());
        public async Task<Trophy[]> FetchTrophiesAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new TrophiesFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<string> FetchTrophyIconsAsync() => await FetchTrophyIconsAsync(new Progress<string>(), new Progress<int>());
        public async Task<string> FetchTrophyIconsAsync(IProgress<string> progressText, IProgress<int> progressPercent)
            => await new TrophyIconsFetcherAction(this).FetchAsync(progressText, progressPercent);

        public async Task<bool> LoginAsync()
        {
            var parameters = new Dictionary<string, string> { { "aimeId", this.AimeId} };
            var encodedContent = new FormUrlEncodedContent(parameters);

            try
            {
                var response = await this.Client.PostAsync(MyPageLoginExecUrl, encodedContent).ConfigureAwait(false);
                this.Logger?.Debug("{Response}", response);

                if (!response.IsSuccessStatusCode)
                {
                    this.Logger?.Error(Localization.Fetcher.ConnectionError);

                    return false;
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                // Check response content HTML to find out if it's an error page.
                var document = new HtmlDocument();
                document.LoadHtml(responseContent);

                var errorCodeNode = document.DocumentNode.SelectSingleNode("//p[@class='error_code']");
                var errorMessageNode = document.DocumentNode.SelectSingleNode("//p[@class='error_message']");

                if (errorCodeNode != null && errorMessageNode != null)
                {
                    var errorCode = errorCodeNode?.InnerText.Trim()
                        .Replace("エラーコード: ", "Error Code: ");
                    var errorMessage = errorMessageNode?.InnerText.Trim();

                    this.Logger?.Error("{ErrorCode}: {ErrorMessage}", errorCode, errorMessage);

                    this.LoginStatus = LoginStatus.LoggedOff;

                    return false;
                }
            }
            catch (Exception ex)
            {
                this.Logger?.Error(Localization.Fetcher.LoginFailed + " {0}", ex.Message);
                this.LoginStatus = LoginStatus.LoggedOff;

                return false;
            }

            this.LoginStatus = LoginStatus.LoggedOn;

            return true;
        }

        public async Task<bool> TryLoginAsync(IProgress<string> progressText)
        {
            for (int tries = 1; tries <= MaxReconnectTries; tries++)
            {
                var result = await this.LoginAsync();
                if (result)
                    return true;

                Log.Error(Localization.Fetcher.ReloggingIn, tries);
                progressText.Report(string.Format(Localization.Fetcher.ReloggingIn, tries));

                await Task.Delay(1000);
            }

            return false;
        }

        public void Dispose()
        {
            this.Client.Dispose();
        }
    }
}