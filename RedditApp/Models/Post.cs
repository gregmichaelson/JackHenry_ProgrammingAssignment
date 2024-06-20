public class Post
{
    string _title { get; set; }
    int _ups { get; set; }
    string _author { get; set; }
    DateTime _timestamp { get; set; }

    public Post(string title, int ups, string author, DateTime timestamp)
    {
        _title = title;
        _ups = ups;
        _author = author;
        _timestamp = timestamp;
    }
    
    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public int Ups
    {
        get { return _ups; }
        set { _ups = value; }
    }

    public string Author
    {
        get { return _author; }
        set { _author = value; }
    }

    public DateTime Timestamp
    {
        get { return _timestamp; }
        set { _timestamp = value; }
    }
}