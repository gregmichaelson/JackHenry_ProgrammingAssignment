public class Poster
{
    string _username { get; set; }
    int _postcount { get; set; }

    public Poster(string username, int postCount)
    {
        _username = username;
        _postcount = postCount;
    }

    public string Username
    {
        get { return _username; }
        set { _username = value; }
    }

    public int PostCount{
        get { return _postcount; }
        set { _postcount = value; }
    }
}