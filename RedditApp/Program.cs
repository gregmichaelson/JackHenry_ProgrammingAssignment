using System;
using System.Reflection.Metadata;
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
        var subredditName = "mildlyinfuriating";
        var sorting = "new";
        var limit = 1000;

        // Create a RedditApiManager instance
        var redditApiManager = new RedditApiManager(clientId, clientSecret, redirectUri);
        var subredditManager = new Subreddit(subredditName);

        // Retrieve posts asynchronously
        //var posts = await redditApiManager.GetPosts(subredditName, sorting, limit);
        await redditApiManager.ContinuouslyGetNewPosts(subredditName, sorting, limit, newPosts =>
        {
            Console.WriteLine(newPosts.Count);
            Console.WriteLine($"Current Interval: {redditApiManager.Interval}");

            subredditManager.AddPost(newPosts);

            foreach (var post in subredditManager.MostUpvotedPosts(3))
            {
                Console.WriteLine($"Title: {post.Title}, Upvotes: {post.Ups}, Author: {post.Author}");
            }
            foreach (var poster in subredditManager.TopPosters(2))
            {
                Console.WriteLine($"Username: {poster.Username} | Number of Posts: {poster.PostCount}");
            }
        });
        //Console.WriteLine(posts.Count);

        //if (posts != null)
        //{
        //    var postsManger = new Subreddit(subredditName);
        //    postsManger.AddPost(posts);

        //    var mostUpvoted = postsManger.MostUpvotedPosts(5);
        //    //Console.WriteLine($"Retrieved {posts.Count} posts from r/{subredditName}:");
        //    foreach (var post in mostUpvoted)
        //    {
        //        Console.WriteLine($" - {post.Title}");
        //        Console.WriteLine($" - {post.Ups}");
        //    }

        //    var topPosters = postsManger.TopPosters(5);

        //    foreach(var poster in topPosters)
        //    {
        //        Console.WriteLine($" - {poster.Username}");
        //        Console.WriteLine($" - {poster.PostCount}");
        //    }
        //}
        //else
        //{
        //    Console.WriteLine("Failed to retrieve posts.");
        //}
    }
}
