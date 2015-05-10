namespace GiphySearch
{

    public class GiphyResult
    {
        public Datum[] data { get; set; }
        public Meta meta { get; set; }
        public Pagination pagination { get; set; }
    }

    public class Meta
    {
        public int status { get; set; }
        public string msg { get; set; }
    }

    public class Pagination
    {
        public int total_count { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
    }

    public class Datum
    {
        public string type { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public string bitly_gif_url { get; set; }
        public string bitly_url { get; set; }
        public string embed_url { get; set; }
        public string username { get; set; }
        public string source { get; set; }
        public string rating { get; set; }
        public string caption { get; set; }
        public string content_url { get; set; }
        public string import_datetime { get; set; }
        public string trending_datetime { get; set; }
        public Images images { get; set; }
    }

    public class Images
    {
        public Fixed_Height fixed_height { get; set; }
        public Fixed_Height_Still fixed_height_still { get; set; }
        public Fixed_Height_Downsampled fixed_height_downsampled { get; set; }
        public Fixed_Width fixed_width { get; set; }
        public Fixed_Width_Still fixed_width_still { get; set; }
        public Fixed_Width_Downsampled fixed_width_downsampled { get; set; }
        public Fixed_Height_Small fixed_height_small { get; set; }
        public Fixed_Height_Small_Still fixed_height_small_still { get; set; }
        public Fixed_Width_Small fixed_width_small { get; set; }
        public Fixed_Width_Small_Still fixed_width_small_still { get; set; }
        public Downsized downsized { get; set; }
        public Downsized_Still downsized_still { get; set; }
        public Downsized_Large downsized_large { get; set; }
        public Original original { get; set; }
        public Original_Still original_still { get; set; }
    }

    public class Fixed_Height
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Fixed_Height_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    public class Fixed_Height_Downsampled
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Fixed_Width
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Fixed_Width_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    public class Fixed_Width_Downsampled
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Fixed_Height_Small
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Fixed_Height_Small_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    public class Fixed_Width_Small
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Fixed_Width_Small_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    public class Downsized
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Downsized_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    public class Downsized_Large
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Original
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string frames { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Original_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

}
