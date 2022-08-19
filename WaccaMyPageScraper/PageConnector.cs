using HtmlAgilityPack;
using Serilog;
using System.Net;
using System.Net.Http.Headers;
using WaccaMyPageScraper.Data;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper
{
    /// <summary>
    ///  The class about a connection to the WACCA My Page.
    /// </summary>
    public class PageConnector : IDisposable
    {
        #region Constants
        private static readonly string MyPageLoginExecUrl = "https://wacca.marv-games.jp/web/login/exec";
        private static readonly string MyPageTopUrl = "https://wacca.marv-games.jp/web";
        private static readonly string MyPageMusicUrl = "https://wacca.marv-games.jp/web/music";
        #endregion

        #region Properties
        internal string AimeId { get; private set; }

        internal HttpClient Client { get; private set; }        

        internal LoginStatus LoginStatus { get; private set; }

        internal ILogger? Logger { get; private set; }
        #endregion

        public PageConnector(string aimeId)
        {
            this.AimeId = aimeId;
            this.Client = new HttpClient();
            this.LoginStatus = LoginStatus.LoggedOff;

            this.Logger = null;
        }

        public PageConnector(string aimeId, ILogger logger)
        {
            this.AimeId = aimeId;
            this.Client = new HttpClient();
            this.LoginStatus = LoginStatus.LoggedOff;

            this.Logger = logger;
        }

        public async Task<bool> LoginAsync()
        {
            var parameters = new Dictionary<string, string> { { "aimeId", this.AimeId} };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await this.Client.PostAsync(MyPageLoginExecUrl, encodedContent).ConfigureAwait(false);
            this.Logger?.Debug("{Response}", response);

            if (!response.IsSuccessStatusCode)
            {
                this.Logger?.Error("Error occured while connecting to the page!");

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
            
            this.LoginStatus = LoginStatus.LoggedOn;

            return true;
        }

        public void Dispose()
        {
            this.Client.Dispose();
        }
    }
}