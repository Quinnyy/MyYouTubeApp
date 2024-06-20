namespace YourTubes.Models
{
    /// <summary>
    /// Class that was created by parsing json response back from search videos API. Custom properties added are playlistID and playlistName (list of strings to store owned playlsit's details)
    /// </summary>
    public class MyYouTube
    {
        public List<string> playlistID { get; set; }
        public List<string> playlistName { get; set; }
        public string? kind { get; set; }
        public string? etag { get; set; }
        public string? nextPageToken { get; set; }
        public string? regionCode { get; set; }
        public Pageinfo? pageInfo { get; set; }
        public Item[]? items { get; set; }

        /// <summary>
        /// Constructor that creates new empty lists when instantiated. This is to avoid null reference exception
        /// </summary>
        public MyYouTube()
        {
            this.items = new Item[0];
            this.playlistName = new List<string>();
            this.playlistID = new List<string>();
        }
    }

    public class Pageinfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class Item
    {
        public string? kind { get; set; }
        public string? etag { get; set; }
        public Id? id { get; set; }
        public Snippet? snippet { get; set; }
    }

    public class Id
    {
        public string? kind { get; set; }
        public string? videoId { get; set; }
    }

    public class Snippet
    {
        public DateTime publishedAt { get; set; }
        public string? channelId { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public Thumbnails? thumbnails { get; set; }
        public string? channelTitle { get; set; }
        public string? liveBroadcastContent { get; set; }
        public DateTime publishTime { get; set; }
    }

    public class Thumbnails
    {
        public Default? _default { get; set; }
        public Medium? medium { get; set; }
        public High? high { get; set; }
    }

    public class Default
    {
        public string? url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Medium
    {
        public string? url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class High
    {
        public string? url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}



