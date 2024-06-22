public class Subreddit
{
    string _name { get; set; }
    List<Post> _posts;
    List<Post> _lastMostUpVotedPosts = new List<Post>();
    List<Poster> _lastTopPosters = new List<Poster>();

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

    public bool HasMostUpvotedPostsChanged(int top)
    {
        var currentMostUpvoted = MostUpvotedPosts(top).ToList();
        if (!_lastMostUpVotedPosts.SequenceEqual(currentMostUpvoted))
        {
            _lastMostUpVotedPosts = currentMostUpvoted;
            return true;
        }
        return false;
    }

    public bool HasTopPostersChanged(int top)
    {
        var currentTopPosters = TopPosters(top);
        if(_lastTopPosters.Count ==0)
        {
            _lastTopPosters = currentTopPosters;
            return true;
        }
        for(int i = 0;  i < currentTopPosters.Count; i++)
        {
            if (currentTopPosters[i].Username != _lastTopPosters[i].Username || currentTopPosters[i].PostCount != _lastTopPosters[i].PostCount)
            {
                _lastTopPosters = currentTopPosters;
                return true;
            }
        }

        return false;
    }
}