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
        #endregion

        #region Private Fields
        private HttpClient _httpClient;
        #endregion

        #region Properties
        internal string AimeId { get; private set; }

        internal LoginStatus LoginStatus { get; private set; }

        internal ILogger? Logger { get; private set; }
        #endregion

        #region Constructors
        public PageConnector(string aimeId)
        {
            this._httpClient = new HttpClient();

            this.AimeId = aimeId;
            this.LoginStatus = LoginStatus.LoggedOff;

            this.Logger = null;
        }

        public PageConnector(string aimeId, ILogger logger)
        {
            this._httpClient = new HttpClient();

            this.AimeId = aimeId;
            this.LoginStatus = LoginStatus.LoggedOff;

            this.Logger = logger;
        }
        #endregion

        public async Task<bool> LoginAsync()
        {
            var parameters = new Dictionary<string, string> { { "aimeId", this.AimeId} };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await this.PostAsync(MyPageLoginExecUrl, encodedContent);
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

        public async Task<string> GetStringAsync(string? requestUri)
        {
            return await this._httpClient.GetStringAsync(requestUri).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> PostAsync(string? requestUri, HttpContent? content)
        {
            return await this._httpClient.PostAsync(requestUri, content).ConfigureAwait(false);
        }

        public bool IsLoggedOn() => this.LoginStatus == LoginStatus.LoggedOn;
        
        public void Dispose()
        {
            this._httpClient.Dispose();
        }
    }
}