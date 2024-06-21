using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class SubredditTests
    {
        [TestMethod]
        public void TestSubredditCreation()
        {
            // Arrange
            const string subredditName = "test_subreddit";

            // Act
            var subreddit = new Subreddit(subredditName);

            // Assert
            Assert.IsNotNull(subreddit);
            Assert.AreEqual(subredditName, subreddit.Name);
            Assert.AreEqual(0, subreddit.MostUpvotedPosts(1).Count()); // Verify empty list initially
        }

        [TestMethod]
        public void TestAddPostsAndMostUpvoted()
        {
            // Arrange
            var subreddit = new Subreddit("test_subreddit");
            var post1 = new Post(title: "Post 1", ups: 10, author: "user1", timestamp: DateTime.UtcNow);
            var post2 = new Post(title: "Post 2", ups: 5, author: "user2", timestamp: DateTime.UtcNow);
            var post3 = new Post(title: "Post 3", ups: 15, author: "user3", timestamp: DateTime.UtcNow);

            // Act
            subreddit.AddPost(post1);
            subreddit.AddPost(post2);
            subreddit.AddPost(post3);

            var topPosts = subreddit.MostUpvotedPosts(2).ToList();

            // Assert
            Assert.AreEqual(2, topPosts.Count);
            Assert.AreEqual(post3, topPosts[0]); // Verify highest upvoted post is first
            Assert.AreEqual(post1, topPosts[1]);
        }

        [TestMethod]
        public void TestTopPosters()
        {
            // Arrange
            var subreddit = new Subreddit("test_subreddit");
            var post1 = new Post(title: "Post 1", ups: 10, author: "user1", timestamp: DateTime.UtcNow);
            var post2 = new Post(title: "Post 2", ups: 5, author: "user1", timestamp: DateTime.UtcNow);
            var post3 = new Post(title: "Post 3", ups: 15, author: "user2", timestamp: DateTime.UtcNow);

            // Act
            subreddit.AddPost(post1);
            subreddit.AddPost(post2);
            subreddit.AddPost(post3);

            var topPosters = subreddit.TopPosters(2).ToList();

            // Assert
            Assert.AreEqual(2, topPosters.Count);
            Assert.AreEqual("user1", topPosters[0].Username); // Verify user with most posts
            Assert.AreEqual(2, topPosters[0].PostCount);
            Assert.AreEqual("user2", topPosters[1].Username);
            Assert.AreEqual(1, topPosters[1].PostCount);
        }


        [TestMethod]
        public void TestAddPostsEnumerable()
        {
            // Arrange
            var subreddit = new Subreddit("test_subreddit");
            var posts = new List<Post>()  {
                new Post(title: "Post 1", ups: 10, author: "user1", timestamp: DateTime.UtcNow),
                new Post(title: "Post 2", ups: 5, author: "user2", timestamp: DateTime.UtcNow),
                new Post(title: "Post 3", ups: 15, author: "user3", timestamp: DateTime.UtcNow)
            };

            // Act
            subreddit.AddPost(posts);

            var topPosts = subreddit.MostUpvotedPosts(2).ToList();

            // Assert
            Assert.AreEqual(2, topPosts.Count);
            Assert.AreEqual(posts[2], topPosts[0]); // Verify highest upvoted post is first
            Assert.AreEqual(posts[0], topPosts[1]);
        }


    }
}