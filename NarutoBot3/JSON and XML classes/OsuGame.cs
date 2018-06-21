#pragma warning disable IDE1006 // Naming Styles

namespace OsuGame
{
    public class BeatMapData
    {
        public string beatmapset_id { get; set; }
        public string beatmap_id { get; set; }
        public int approved { get; set; }
        public int total_length { get; set; }
        public string hit_length { get; set; }
        public string version { get; set; }
        public string file_md5 { get; set; }
        public string diff_size { get; set; }
        public string diff_overall { get; set; }
        public string diff_approach { get; set; }
        public string diff_drain { get; set; }
        public string mode { get; set; }
        public string approved_date { get; set; }
        public string last_update { get; set; }
        public string artist { get; set; }
        public string title { get; set; }
        public string creator { get; set; }
        public string bpm { get; set; }
        public string source { get; set; }
        public string tags { get; set; }
        public string genre_id { get; set; }
        public string language_id { get; set; }
        public string favourite_count { get; set; }
        public string playcount { get; set; }
        public string passcount { get; set; }
        public string max_combo { get; set; }
        public string difficultyrating { get; set; }
    }

    public class ScoreData
    {
        public string score_id { get; set; }
        public string score { get; set; }
        public string username { get; set; }
        public string maxcombo { get; set; }
        public string count50 { get; set; }
        public string count100 { get; set; }
        public string count300 { get; set; }
        public string countmiss { get; set; }
        public string countkatu { get; set; }
        public string countgeki { get; set; }
        public string perfect { get; set; }
        public string enabled_mods { get; set; }
        public string user_id { get; set; }
        public string date { get; set; }
        public string rank { get; set; }
        public decimal? pp { get; set; }
        public string replay_available { get; set; }
    }


}