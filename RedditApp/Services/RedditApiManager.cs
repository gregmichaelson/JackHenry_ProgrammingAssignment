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
    private HashSet<string> _retrievedPostIds;
    private TimeSpan _interval { get; set; }
    public Subreddit SubredditManager { get; private set; }

    public RedditApiManager(string clientId, string clientSecret, string redirectUri, string subredditName)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _redirectUri = redirectUri;
        _baseUrl = "https://oauth.reddit.com/r/";
        _retrievedPostIds = new HashSet<string>();
        SubredditManager = new Subreddit(subredditName);
    }

    public TimeSpan Interval
    {
        get { return _interval; }
        set { _interval = value; }
    }

    public async Task<List<Post>> GetPosts(string subreddit, string sorting, int limit)
    {
        if (_accessToken == null)
        {
            _accessToken = await GetAuthorizationToken();
        }

        var posts = new List<Post>();
        string after = null;

        while (limit == 0 || posts.Count < limit)
        {
            var remainingPosts = limit - posts.Count;
            var batchSize = remainingPosts > 100 ? 100 : remainingPosts;
            var url = $"{_baseUrl}{subreddit}/{sorting}.json?limit={batchSize}&after={after}";

            //Get All Posts
            if (limit == 0)
            {
                url = $"{_baseUrl}{subreddit}/{sorting}.json?after={after}";
            }


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
                    var postId = post.GetProperty("data").GetProperty("id").GetString();
                    if (!_retrievedPostIds.Contains(postId))
                    {
                        var postData = JsonSerializer.Deserialize<Post>(post.GetProperty("data").ToString(), options);
                        posts.Add(postData);
                        _retrievedPostIds.Add(postId);

                    }
                }

                after = jsonDocument.RootElement.GetProperty("data").GetProperty("after").GetString();
                if (string.IsNullOrEmpty(after))
                {
                    break; // No more posts to fetch
                }

                // Calculate the interval for the next request
                _interval = CalculateInterval(response.Headers);
                await Task.Delay(_interval);
            }
            else
            {
                Console.WriteLine($"Error: Failed to retrieve posts. Status code: {response.StatusCode}");
                break;
            }
        }

        return posts;
    }

    private TimeSpan CalculateInterval(HttpResponseHeaders headers)
    {
        if (headers.TryGetValues("x-ratelimit-remaining", out var remainingValues) &&
            headers.TryGetValues("x-ratelimit-reset", out var resetValues))
        {
            if (int.TryParse(remainingValues.FirstOrDefault(), out int remaining) &&
                int.TryParse(resetValues.FirstOrDefault(), out int reset))
            {
                if (remaining > 0)
                {
                    return TimeSpan.FromSeconds(reset / (double)remaining);
                }
                else
                {
                    return TimeSpan.FromSeconds(reset);
                }
            }
        }
        return TimeSpan.FromSeconds(1); // Default interval if headers are missing
    }

    public async Task ContinuouslyGetNewPosts(string subreddit, string sorting, int limit, Action<List<Post>> onNewPosts)
    {
        while (true)
        {
            var newPosts = await GetPosts(subreddit, sorting, limit);
            if (newPosts != null && newPosts.Count > 0)
            {
                SubredditManager.AddPost(newPosts);
                onNewPosts(newPosts);
            }
            await Task.Delay(_interval);
        }
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
