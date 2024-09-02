using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace BMK.Api.Controllers.Api.UserModule
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LinkedInController : ControllerBase
    {
        // LinkedIn API endpoints
        private const string AUTH_ENDPOINT = "https://www.linkedin.com/oauth/v2/authorization";
        private const string TOKEN_ENDPOINT = "https://www.linkedin.com/oauth/v2/accessToken";
        private const string PROFILE_PIC_ENDPOINT = "https://api.linkedin.com/v2/me";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly WebClient _client;
        // Method to obtain an access token using OAuth 2.0
        public LinkedInController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _client = new WebClient();
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            string clientId = _configuration["LinkedIn:ClientId"];
            string redirectUri = _configuration["LinkedIn:RedirectUri"];
            string clientSecret = _configuration["LinkedIn:ClientSecret"];
            string state = Guid.NewGuid().ToString("N");

            string authorizationUrl = $"https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id={clientId}&client_secret={clientSecret}&redirect_uri={redirectUri}&state={state}&scope=r_liteprofile";

            return Redirect(authorizationUrl);
        }
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery]string code, [FromQuery]string state)
        {
            string clientId = _configuration["LinkedIn:ClientId"];
            string clientSecret = _configuration["LinkedIn:ClientSecret"];
            string redirectUri = _configuration["LinkedIn:RedirectUri"];

            // Verify state if necessary

            using var client = _httpClientFactory.CreateClient();
            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "authorization_code"},
            {"code", code},
            {"redirect_uri", redirectUri},
            {"client_id", clientId},
            {"client_secret", clientSecret}
        });

            var tokenResponse = await client.PostAsync("https://www.linkedin.com/oauth/v2/accessToken", tokenRequest);
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokenJson = JsonConvert.DeserializeObject<dynamic>(tokenContent);

            string accessToken = tokenJson.access_token;

            // Store or use the access token as needed
            return Ok(tokenJson);
        }

        [HttpGet("profile-picture")]
        public async Task<IActionResult> GetProfilePicture(string accessToken)
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = await client.GetAsync("https://api.linkedin.com/v2/me?projection=(profilePicture(displayImage~:playableStreams))");
                    response.EnsureSuccessStatusCode();

                    var profileContent = await response.Content.ReadAsStringAsync();
                    var profileData = JsonDocument.Parse(profileContent);

                    // Extract image URL logic

                    var profilePictureUrl = profileData.RootElement.GetProperty("profilePicture")
                        .GetProperty("displayImage~")
                        .GetProperty("elements")[0]
                        .GetProperty("identifiers")[0]
                        .GetProperty("identifier")
                        .GetString();


                    //var profilePictureUrl = $"https://media.licdn.com/dms/image/{profilePictureUrn}/profile-picture-url.jpg";

                    return Ok(profilePictureUrl);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}

