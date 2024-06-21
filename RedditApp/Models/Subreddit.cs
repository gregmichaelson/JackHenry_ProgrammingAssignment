public class Subreddit
{
    string _name { get; set; }
    List<Post> _posts;

    public Subreddit(string name)
    {
        _name = name;
        _posts = new List<Post>();
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public void AddPost(Post post)
    {
        _posts.Add(post);
    }

    public void AddPost(IEnumerable<Post> posts)
    {
        _posts.AddRange(posts);
    }

    public IEnumerable<Post> MostUpvotedPosts(int top)
    {
        return _posts.OrderByDescending(post => post.Ups).Take(top);
    }

    public List<Poster> TopPosters(int top)
    {
        return _posts.GroupBy(post => post.Author)
                     .Select(group => new Poster(group.Key, group.Count()))
                     .Where(poster => poster.Username != "[deleted]")
                     .OrderByDescending(poster => poster.PostCount)
                     .Take(top)
                     .ToList();
    }
}