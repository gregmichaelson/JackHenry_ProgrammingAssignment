public class PostStats
{
    Post _mostupvoted { get; set; }
    string _topposter { get; set; }
    int _postcount { get; set; }

    public PostStats(Post mostupvoted, string topposter, int postcount)
    {
        _mostupvoted = mostupvoted;
        _topposter = topposter;
        _postcount = postcount;
    }

    public Post MostUpVoted
    {
        get { return _mostupvoted; }
        set { _mostupvoted = value; }
    }

    public string TopPoster
    {
        get { return _topposter; }
        set { _topposter = value; }
    }

    public int PostCount{
        get { return _postcount; }
        set { _postcount = value; }
    }

}