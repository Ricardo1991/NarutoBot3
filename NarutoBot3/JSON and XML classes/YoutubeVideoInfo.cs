namespace YoutubeVideoInfo
{

    public class YoutubeVideoInfo
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public Pageinfo pageInfo { get; set; }
        public Item[] items { get; set; }
    }

    public class Pageinfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public Snippet snippet { get; set; }
        public Contentdetails contentDetails { get; set; }
        public Statistics statistics { get; set; }
    }

    public class Snippet
    {
        public System.DateTime publishedAt { get; set; }
        public string channelId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Thumbnails thumbnails { get; set; }
        public string channelTitle { get; set; }
        public string categoryId { get; set; }
        public string liveBroadcastContent { get; set; }
    }

    public class Thumbnails
    {
        public Default _default { get; set; }
        public Medium medium { get; set; }
        public High high { get; set; }
        public Standard standard { get; set; }
        public Maxres maxres { get; set; }
    }

    public class Default
    {
        public System.Uri url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Medium
    {
        public System.Uri url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class High
    {
        public System.Uri url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Standard
    {
        public System.Uri url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Maxres
    {
        public System.Uri url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Contentdetails
    {
        public string duration { get; set; }
        public string dimension { get; set; }
        public string definition { get; set; }
        public string caption { get; set; }
        public bool licensedContent { get; set; }
    }

    public class Statistics
    {
        public string viewCount { get; set; }
        public string likeCount { get; set; }
        public string dislikeCount { get; set; }
        public string favoriteCount { get; set; }
        public string commentCount { get; set; }
    }


}
