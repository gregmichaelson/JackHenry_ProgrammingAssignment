using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Retrieve Reddit API credentials and settings from configuration
        var redditConfig = configuration.GetSection("Reddit").Get<RedditConfig>();
        var subredditConfig = configuration.GetSection("Subreddit").Get<SubredditConfig>();

        // Create a RedditApiManager instance
        var redditApiManager = new RedditApiManager(redditConfig.ClientId, redditConfig.ClientSecret, redditConfig.RedirectUri, subredditConfig.Name);
        //var subredditManager = new Subreddit(subredditConfig.Name);

        // Retrieve posts asynchronously
        await redditApiManager.ContinuouslyGetNewPosts(subredditConfig.Name, subredditConfig.Sorting, subredditConfig.Limit, newPosts =>
        {
            Console.WriteLine("Press Ctrl + C to stop application");
            Console.WriteLine($"Subreddit: {redditApiManager.SubredditManager.Name}");
            Console.WriteLine($"New posts count: {newPosts.Count}");
            Console.WriteLine($"Current Interval: {redditApiManager.Interval}");

            if (redditApiManager.SubredditManager.HasMostUpvotedPostsChanged(3))
            {
                Console.WriteLine("Most upvoted posts have changed:");
                foreach (var post in redditApiManager.SubredditManager.MostUpvotedPosts(3))
                {
                    Console.WriteLine($"Title: {post.Title}, Upvotes: {post.Ups}, Author: {post.Author}");
                }
            }
            else
            {
                Console.WriteLine("No changes to the posts with the most upvotes");
            }

            if (redditApiManager.SubredditManager.HasTopPostersChanged(2))
            {
                Console.WriteLine("Top posters have changed:");
                foreach (var poster in redditApiManager.SubredditManager.TopPosters(2))
                {
                    Console.WriteLine($"Username: {poster.Username} | Number of Posts: {poster.PostCount}");
                }
            }
            else
            {
                Console.WriteLine("No changes to the top posters.");
            }
        });
        
    }
    public class RedditConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }

    public class SubredditConfig
    {
        public string Name { get; set; }
        public string Sorting { get; set; }
        public int Limit { get; set; }
    }
}
