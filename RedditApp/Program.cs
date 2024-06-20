using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Replace with your actual Reddit credentials (avoid storing them directly in code)
        var clientId = "5YWiW6qmxDv8XdXeJwjkKA";
        var clientSecret = "HAIlLqLJv_dp3BCJPMtRUrF-R9BvyA";
        var redirectUri = "http://localhost:5000/";

        // Optional: Customize these parameters if needed
        var subredditName = "programming";
        var sorting = "new";
        var limit = 1000;

        // Create a RedditApiManager instance
        var redditApiManager = new RedditApiManager(clientId, clientSecret, redirectUri);

        // Retrieve posts asynchronously
        var posts = await redditApiManager.GetPosts(subredditName, sorting, limit);

        if (posts != null)
        {
            var postsManger = new Subreddit(subredditName);
            postsManger.AddPost(posts);

            var mostUpvoted = postsManger.MostUpvotedPosts(5);
            //Console.WriteLine($"Retrieved {posts.Count} posts from r/{subredditName}:");
            foreach (var post in mostUpvoted)
            {
                Console.WriteLine($" - {post.Title}");
                Console.WriteLine($" - {post.Ups}");
            }

            var topPosters = postsManger.TopPosters(5);

            foreach(var poster in topPosters)
            {
                Console.WriteLine($" - {poster.Username}");
                Console.WriteLine($" - {poster.PostCount}");
            }
        }
        else
        {
            Console.WriteLine("Failed to retrieve posts.");
        }
    }
}
