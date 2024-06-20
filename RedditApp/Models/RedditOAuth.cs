using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class RedditOAuth
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly string _state;

    public RedditOAuth(string clientId, string clientSecret, string redirectUri)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _redirectUri = redirectUri;
        _state = GenerateRandomState();
    }

    public string GetAuthorizationUrl()
    {
        return $"https://www.reddit.com/api/v1/authorize?client_id={_clientId}&response_type=code&state={_state}&redirect_uri={_redirectUri}&duration=temporary&scope=read identity report&t=day";
    }

    public void OpenBrowser(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open browser: {ex.Message}");
        }
    }

    public async Task<string> CaptureAuthorizationCode()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add(_redirectUri);
        listener.Start();

        Console.WriteLine("Waiting for authorization code...");

        var context = await listener.GetContextAsync();
        var response = context.Response;

        var uri = context.Request.Url;
        var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);

        if (queryParams["state"] == _state && !string.IsNullOrEmpty(queryParams["code"]))
        {
            var code = queryParams["code"];
            var responseString = "<html><body>You may close this window.</body></html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Close();

            listener.Stop();
            return code;
        }
        else
        {
            listener.Stop();
            return null;
        }
    }

    public async Task<string> GetAccessTokenAsync(string code)
    {
        using var client = new HttpClient();
        var basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
        client.DefaultRequestHeaders.Add("Authorization", $"Basic {basicAuthValue}");
        client.DefaultRequestHeaders.Add("User-Agent", "RedditApiTest/1.0 by GM_DragonMage");

        var parameters = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", _redirectUri }
            };

        var content = new FormUrlEncodedContent(parameters);
        var response = await client.PostAsync("https://www.reddit.com/api/v1/access_token", content);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var json = JObject.Parse(responseContent);
            return json["access_token"].ToString();
        }
        else
        {
            Console.WriteLine($"Response Content: {responseContent}");
            throw new HttpRequestException($"Failed to retrieve access token: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }

    private static string GenerateRandomState()
    {
        return Guid.NewGuid().ToString();
    }
}
