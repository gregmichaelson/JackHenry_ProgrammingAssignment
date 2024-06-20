using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class RedditApiManager
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly string _baseUrl;
    private string _accessToken;

    public RedditApiManager(string clientId, string clientSecret, string redirectUri)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _redirectUri = redirectUri;
        _baseUrl = "https://oauth.reddit.com/r/";
    }

    public async Task<List<Post>> GetPosts(string subreddit, string sorting, int limit)
    {
        if (_accessToken == null)
        {
            _accessToken = await GetAuthorizationToken();
        }

        var posts = new List<Post>();
        string after = null;

        while (posts.Count < limit)
        {
            var remainingPosts = limit - posts.Count;
            var batchSize = remainingPosts > 100 ? 100 : remainingPosts;

            var url = $"{_baseUrl}{subreddit}/{sorting}.json?limit={batchSize}&after={after}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RedditApiTest/1.0 by GM_DragonMage");

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var jsonDocument = await JsonSerializer.DeserializeAsync<JsonDocument>(stream, options);

                foreach (var post in jsonDocument.RootElement.GetProperty("data").GetProperty("children").EnumerateArray())
                {
                    posts.Add(JsonSerializer.Deserialize<Post>(post.GetProperty("data").ToString(), options));
                }

                after = jsonDocument.RootElement.GetProperty("data").GetProperty("after").GetString();
                if (string.IsNullOrEmpty(after))
                {
                    break; // No more posts to fetch
                }
            }
            else
            {
                Console.WriteLine($"Error: Failed to retrieve posts. Status code: {response.StatusCode}");
                break;
            }
        }

        return posts;
    }

    private async Task<string> GetAuthorizationToken()
    {
        var redditOAuth = new RedditOAuth(_clientId, _clientSecret, _redirectUri);

        string authUrl = redditOAuth.GetAuthorizationUrl();
        Console.WriteLine("Opening browser to perform OAuth authentication...");
        redditOAuth.OpenBrowser(authUrl);

        var code = await redditOAuth.CaptureAuthorizationCode();

        if (!string.IsNullOrEmpty(code))
        {
            try
            {
                var accessToken = await redditOAuth.GetAccessTokenAsync(code);
                return accessToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obtaining access token: {ex.Message}");
                return null;
            }
        }
        else
        {
            Console.WriteLine("Failed to obtain authorization code.");
            return null;
        }
    }
}
