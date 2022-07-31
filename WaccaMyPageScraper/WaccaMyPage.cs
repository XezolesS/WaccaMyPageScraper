using HtmlAgilityPack;
using System.Net;
using System.Net.Http.Headers;

using WaccaMyPageScraper.Data;

namespace WaccaMyPageScraper
{
    public class WaccaMyPage
    {
        private static readonly string MyPageLoginExecUrl = "https://wacca.marv-games.jp/web/login/exec";

        private static readonly string MyPageTopUrl = "https://wacca.marv-games.jp/web";
        private static readonly string MyPageMusicUrl = "https://wacca.marv-games.jp/web/music";

        public HttpClient Client { get; private set; }

        public string AimeId { get; private set; }

        public WaccaMyPage(string aimeId)
        {
            this.Client = new HttpClient();
            this.AimeId = aimeId;
        }

        public async Task<bool> LoginAsync()
        {
            var parameters = new Dictionary<string, string> { { "aimeId", this.AimeId} };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await this.Client.PostAsync(MyPageLoginExecUrl, encodedContent).ConfigureAwait(false);

            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<User> FetchUserAsync()
        {
            // Get a response from "My Page Top".
            var response = await this.CallMyPage();

            var document = new HtmlDocument();
            document.LoadHtml(response);

            // Select user data from HTML and convert to the data.
            var userInfoNodes = document.DocumentNode.SelectSingleNode("//section[@class='user-info']");

            var userIconNode = userInfoNodes.SelectSingleNode("./div[@class='user-info__icon']");
            var userDetailNode = userInfoNodes.SelectSingleNode("./div[@class='user-info__detail']");

            var stageIconSrc = userIconNode.SelectSingleNode("./div[@class='user-info__icon__stage']/img").Attributes["src"].Value;
            var stageIconNumbers = stageIconSrc.Split('/').Last()
                .Replace("stage_icon_", "")
                .Replace(".png", "")
                .Split('_');

            var name = userDetailNode.SelectSingleNode("./div/div[@class='user-info__detail__name']").InnerText;
            var level = userDetailNode.SelectSingleNode("./div/div[@class='user-info__detail__lv']/span").InnerText;
            var rating = userDetailNode.SelectSingleNode("./div[@class='rating__wrap']/div/div").InnerText;

            return new User
            {
                Name = name,
                Level = int.Parse(level.Replace("Lv.", "")),
                Rate = int.Parse(rating),
                StageCleared = int.Parse(stageIconNumbers[0]),
                StageGrade = int.Parse(stageIconNumbers[1])
            };
        }

        public async Task<string> CallMyPage()
        {
            return await this.Client.GetStringAsync(MyPageTopUrl);
        }
    }
}